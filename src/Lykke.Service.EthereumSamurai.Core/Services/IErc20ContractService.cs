using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;

namespace Lykke.Service.EthereumSamurai.Core.Services
{
    public interface IErc20ContractService
    {
        Task<IEnumerable<Erc20ContractModel>> GetAsync(Erc20ContractQuery query);
    }
}