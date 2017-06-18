using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EthereumSamurai.Indexer.Jobs
{
    public interface IJob
    {
        Task RunAsync();
        Task RunAsync(CancellationToken cancellationToken);
    }
}
