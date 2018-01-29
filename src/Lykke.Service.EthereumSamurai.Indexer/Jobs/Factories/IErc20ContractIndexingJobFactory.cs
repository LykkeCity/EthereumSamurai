namespace Lykke.Service.EthereumSamurai.Indexer.Jobs
{
    public interface IErc20ContractIndexingJobFactory
    {
        IJob GetJob();
    }
}