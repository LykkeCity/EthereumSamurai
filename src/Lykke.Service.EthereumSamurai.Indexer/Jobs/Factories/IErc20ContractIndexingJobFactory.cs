namespace Lykke.Job.EthereumSamurai.Jobs
{
    public interface IErc20ContractIndexingJobFactory
    {
        IJob GetJob();
    }
}