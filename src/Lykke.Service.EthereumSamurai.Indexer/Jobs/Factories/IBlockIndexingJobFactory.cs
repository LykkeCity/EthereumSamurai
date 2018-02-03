using Lykke.Service.EthereumSamurai.Core.Models;

namespace Lykke.Job.EthereumSamurai.Jobs
{
    public interface IBlockIndexingJobFactory
    {
        IJob GetJob(IIndexingSettings settings);
    }
}