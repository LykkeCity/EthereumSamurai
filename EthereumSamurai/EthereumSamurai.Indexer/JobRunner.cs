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
        private readonly ILogger _logger;

        public JobRunner(IEnumerable<IJob> jobs, ILogger logger)
        {
            _logger = logger;
            _jobs = jobs;
            _runningTasks = new List<Task>(jobs.Count());
        }

        public async Task RunTasks(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _runningTasks = _jobs.Select(job => job.RunAsync(cancellationToken));

            await WaitAll();
        }

        private async Task WaitAll()
        {
            try
            {
                await Task.WhenAll(_runningTasks).ConfigureAwait(false);
                _logger.LogInformation($"Jobs has been completed");
            }
            catch (OperationCanceledException e)
            {
                _logger.LogInformation($"Jobs has been canceled");
            }
            catch (Exception e)
            {
                _logger.LogError(null, e, e.Message);
                await RunTasks(_cancellationToken);
            }
        }
    }
}
