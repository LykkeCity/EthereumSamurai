namespace EthereumSamurai.Indexer.Jobs
{
    public interface IErc20ContractIndexingJobFactory
    {
        IJob GetJob();
    }
}