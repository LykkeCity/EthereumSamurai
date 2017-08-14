using EthereumSamurai.Core.Models;

namespace EthereumSamurai.Indexer.Jobs
{
    public interface IErc20BalanceIndexingJobFactory
    {
        IJob GetJob(ulong startFrom);
    }
}