using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;

namespace EthereumSamurai.Core.Services
{
    public interface IErc20BalanceService
    {
        Task<IEnumerable<Erc20BalanceModel>> GetAsync(Erc20BalanceQuery query);
        
        Task<ulong?> GetNextBlockToIndexAsync(ulong startFrom);

        Task IndexBlockAsync(ulong blockNumber, int jobVersion);
    }
}