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

        public JobRunner(IEnumerable<IJob> jobs, ILog logger)
        {
            _logger = logger;
            _jobs = jobs;
            _runningTasks = new List<Task>(jobs.Count());
        }

        public async Task RunTasks(CancellationToken cancellationToken)
        {
            _taskCancellationDictionary = new Dictionary<Task, CancellationTokenSource>();
            _cancellationToken = cancellationToken;
            _runningTasks = _jobs.Select(job =>
            {
                CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                CancellationToken linkedToken = cts.Token;
                var task = job.RunAsync(linkedToken);
                _taskCancellationDictionary[task] = cts;

                return task;
            });

            await WaitAll();
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
    }
}
