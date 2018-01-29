using Lykke.Service.EthereumSamurai.Core.Models;

namespace Lykke.Service.EthereumSamurai.Indexer.Jobs
{
    public interface IBlockIndexingJobFactory
    {
        IJob GetJob(IIndexingSettings settings);
    }
}