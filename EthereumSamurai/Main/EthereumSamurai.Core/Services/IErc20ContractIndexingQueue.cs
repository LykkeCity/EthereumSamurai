using EthereumSamurai.Models.Blockchain;

namespace EthereumSamurai.Core.Services
{
    public interface IErc20ContractIndexingQueue
    {
        DeployedContractModel Dequeue();

        void Enqueue(DeployedContractModel contract);
    }
}