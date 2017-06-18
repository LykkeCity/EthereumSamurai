using EthereumSamurai.Indexer.Jobs;
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
        private IEnumerable<Task> _runningTasks { get; set; }

        public JobRunner(IEnumerable<IJob> jobs)
        {
            _jobs = jobs;
            _runningTasks = new List<Task>(jobs.Count());
        }

        public async Task RunTasks(CancellationToken cancellationToken)
        {
            _runningTasks = _jobs.Select(job => job.RunAsync(cancellationToken));

            await WaitAll();
        }

        private async Task WaitAll()
        {
            try
            {
                await Task.WhenAll(_runningTasks).ConfigureAwait(false);
            }
            catch (OperationCanceledException e)
            {

            }
            catch (Exception e)
            {

            }
        }
    }
}
