using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;

namespace EthereumSamurai.Core.Services
{
    public interface IErc20ContractService
    {
        Task<IEnumerable<Erc20ContractModel>> GetAsync(Erc20ContractQuery query);
    }
}