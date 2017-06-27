using EthereumSamurai.Core.Services;
using EthereumSamurai.Indexer.Jobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EthereumSamurai.Indexer
{
    public class JobRunner
    {
        private readonly IEnumerable<IJob> _jobs;
        private IEnumerable<Task> _runningTasks;
        public CancellationToken _cancellationToken;
        private readonly ILog _logger;
        private Dictionary<Task, CancellationTokenSource> _taskCancellationDictionary;
        private Dictionary<Task, IJob> _taskJobDictionary;

        public JobRunner(IEnumerable<IJob> jobs, ILog logger)
        {
            _taskCancellationDictionary = new Dictionary<Task, CancellationTokenSource>();
            _taskJobDictionary = new Dictionary<Task, IJob>();
            _logger = logger;
            _jobs = jobs;
            _runningTasks = new List<Task>(jobs.Count());
        }

        public async Task RunTasks(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _runningTasks = _jobs.Select(job =>
            {
                return RunJob(job);
            });

            await WaitAndRetryAll();
        }

        private Task RunJob(IJob job)
        {
            var runningTask = _taskJobDictionary.Where(x => x.Value == job).Select(y => y.Key).FirstOrDefault();
            if (runningTask == null || runningTask.IsFaulted)
            {
                CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken);
                CancellationToken linkedToken = cts.Token;
                if (runningTask != null)
                {
                    _taskCancellationDictionary.Remove(runningTask);
                    _taskJobDictionary.Remove(runningTask);
                }

                runningTask = job.RunAsync(linkedToken);
                _taskCancellationDictionary[runningTask] = cts;
                _taskJobDictionary[runningTask] = job;
            }

            return runningTask;
        }

        private async Task WaitAndRetryAll()
        {
            try
            {
                await Task.WhenAll(_runningTasks).ConfigureAwait(false);
                await _logger.WriteInfoAsync("JobRunner", "WaitAndRetryAll", "", "Jobs has been completed", DateTime.UtcNow);
            }
            catch (OperationCanceledException e)
            {
                await _logger.WriteInfoAsync("JobRunner", "WaitAndRetryAll", "", "Jobs has been cancelled", DateTime.UtcNow);
            }
            catch (Exception e)
            {
                await _logger.WriteErrorAsync("JobRunner", "WaitAndRetryAll", "Error during indexing. Retry", e, DateTime.UtcNow);
                await Task.Delay(1000);
                await RunTasks(_cancellationToken);
            }
        }

        private async Task WaitAll()
        {
            try
            {
                await Task.WhenAll(_runningTasks).ConfigureAwait(false);
                await _logger.WriteInfoAsync("JobRunner", "WaitAll", "", "Jobs has been completed", DateTime.UtcNow);
            }
            catch (OperationCanceledException e)
            {
                await _logger.WriteInfoAsync("JobRunner", "WaitAll", "", "Jobs has been cancelled", DateTime.UtcNow);
            }
            catch (Exception e)
            {
                _runningTasks?.ToList().ForEach(task =>
                {
                    _taskCancellationDictionary[task].Cancel();
                });
                await _logger.WriteErrorAsync("JobRunner", "WaitAll", "Error during indexing. Retry", e, DateTime.UtcNow);
                await Task.Delay(1000);
                await RunTasks(_cancellationToken);
            }
        }

        private async Task Wait(Task taskToWait)
        {
            try
            {
                await taskToWait.ConfigureAwait(false);
                await _logger.WriteInfoAsync("JobRunner", "Wait", "", "Job has been completed", DateTime.UtcNow);
            }
            catch (OperationCanceledException e)
            {
                await _logger.WriteInfoAsync("JobRunner", "Wait", "", "Job has been cancelled", DateTime.UtcNow);
            }
            catch (Exception e)
            {
                _taskCancellationDictionary[taskToWait].Cancel();
                await _logger.WriteErrorAsync("JobRunner", "Wait", "Error during indexing. Retry", e, DateTime.UtcNow);
                await Task.Delay(1000);
                await RunTasks(_cancellationToken);
            }
        }
    }
}
