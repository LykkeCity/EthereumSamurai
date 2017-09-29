using System.Threading.Tasks;
using EthereumSamurai.Models.Blockchain;

namespace EthereumSamurai.Core.Services
{
    public interface IErc20ContractIndexingService
    {
        Task<DeployedContractModel> GetNextContractToIndexAsync();

        Task IndexContractAsync(DeployedContractModel deployedContract);
    }
}