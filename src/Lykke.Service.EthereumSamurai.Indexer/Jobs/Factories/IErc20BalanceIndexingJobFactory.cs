using Lykke.Service.EthereumSamurai.Core.Models;

namespace Lykke.Service.EthereumSamurai.Indexer.Jobs
{
    public interface IErc20BalanceIndexingJobFactory
    {
        IJob GetJob(ulong startFrom);
    }
}