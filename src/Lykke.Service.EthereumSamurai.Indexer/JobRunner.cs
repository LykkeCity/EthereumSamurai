using Common.Log;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Job.EthereumSamurai.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.Job.EthereumSamurai
{
    public class JobRunner
    {
        private readonly IEnumerable<IJob>                         _jobs;
        private readonly ILog                                      _logger;
        private readonly Dictionary<IJob, bool>                    _completionDictionary;
        private readonly object                                    _locker = new object();
        private readonly Dictionary<Task, CancellationTokenSource> _taskCancellationDictionary;
        private readonly Dictionary<Task, IJob>                    _taskJobDictionary;

        private CancellationToken _cancellationToken;
        private IEnumerable<Task> _runningTasks;

        public JobRunner(IEnumerable<IJob> jobs, ILog logger)
        {
            _completionDictionary       = new Dictionary<IJob, bool>();
            _jobs                       = jobs;
            _logger                     = logger;
            _runningTasks               = new List<Task>(jobs.Count());
            _taskCancellationDictionary = new Dictionary<Task, CancellationTokenSource>();
            _taskJobDictionary          = new Dictionary<Task, IJob>();
        }

        public async Task RunTasks(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            await WaitAndRetryAll();
        }
        
        private Task RunJob(IJob job)
        {
            var runningTask = _taskJobDictionary.Where(x => x.Value == job).Select(y => y.Key).FirstOrDefault();
            if (runningTask == null || runningTask.IsFaulted)
            {
                var cts         = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken);
                var linkedToken = cts.Token;

                if (runningTask != null)
                {
                    _taskCancellationDictionary.Remove(runningTask);
                    _taskJobDictionary.Remove(runningTask);
                }

                runningTask                              = job.RunAsync(linkedToken);
                _taskCancellationDictionary[runningTask] = cts;
                _taskJobDictionary[runningTask]          = job;
            }
            else if (runningTask.Status == TaskStatus.RanToCompletion || runningTask.Status == TaskStatus.Canceled)
            {
                var completedJob = _taskJobDictionary[runningTask];

                _completionDictionary[completedJob] = true;

                _taskCancellationDictionary.Remove(runningTask);
                _taskJobDictionary.Remove(runningTask);

                return null;
            }

            return runningTask;
        }

        private async Task<IEnumerable<Task>> RunJobs(CancellationToken cancellationToken)
        {
            lock (_locker)
            {
                _cancellationToken = cancellationToken;
                _runningTasks      = _jobs.Select(job =>
                {

                    if (!_completionDictionary.ContainsKey(job))
                    {
                        return RunJob(job);
                    }
                    
                    return null;

                }).Where(task => task != null);
            }

            return _runningTasks.ToList();
        }
        
        private async Task WaitAndRetryAll()
        {
            IEnumerable<Task> currentlyRunning;

            do
            {
                currentlyRunning = await RunJobs(_cancellationToken);
                try
                {
                    Task.WaitAny(currentlyRunning.ToArray(), _cancellationToken);
                }
                catch (OperationCanceledException e)
                {
                    await _logger.WriteInfoAsync("JobRunner", "WaitAndRetryAll", "", "Jobs has been cancelled", DateTime.UtcNow);
                }
                catch (Exception e)
                {
                    await _logger.WriteErrorAsync("JobRunner", "WaitAndRetryAll", "Error during indexing. Retry", e, DateTime.UtcNow);

                    await Task.Delay(1000, _cancellationToken);
                }
            } while 
            (
                !_cancellationToken.IsCancellationRequested
             && currentlyRunning.Count(x => x.Status == TaskStatus.RanToCompletion) !=
                currentlyRunning.Count() && currentlyRunning.Count() != 0
            );
            
            await _logger.WriteInfoAsync("JobRunner", "WaitAndRetryAll", "", "Jobs has been completed", DateTime.UtcNow);
        }
    }
}