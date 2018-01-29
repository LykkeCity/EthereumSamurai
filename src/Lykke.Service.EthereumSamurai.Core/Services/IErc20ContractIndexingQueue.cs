using Lykke.Service.EthereumSamurai.Models.Blockchain;

namespace Lykke.Service.EthereumSamurai.Core.Services
{
    public interface IErc20ContractIndexingQueue
    {
        DeployedContractModel Dequeue();

        void Enqueue(DeployedContractModel contract);
    }
}