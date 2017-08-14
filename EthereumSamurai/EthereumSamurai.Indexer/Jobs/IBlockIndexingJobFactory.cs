using EthereumSamurai.Core.Models;

namespace EthereumSamurai.Indexer.Jobs
{
    public interface IBlockIndexingJobFactory
    {
        IJob GetJob(IIndexingSettings settings);
    }
}