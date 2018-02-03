using Lykke.Service.EthereumSamurai.Core.Models;

namespace Lykke.Job.EthereumSamurai.Jobs
{
    public interface IErc20BalanceIndexingJobFactory
    {
        IJob GetJob(ulong startFrom);
    }
}