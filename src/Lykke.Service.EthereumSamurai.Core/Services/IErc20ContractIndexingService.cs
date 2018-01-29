using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models.Blockchain;

namespace Lykke.Service.EthereumSamurai.Core.Services
{
    public interface IErc20ContractIndexingService
    {
        Task<DeployedContractModel> GetNextContractToIndexAsync();

        Task IndexContractAsync(DeployedContractModel deployedContract);
    }
}