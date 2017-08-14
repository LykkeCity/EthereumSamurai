using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumSamurai.Models.Blockchain;

namespace EthereumSamurai.Core.Services
{
    public interface IErc20BalanceService
    {
        Task<IEnumerable<Erc20BalanceModel>> GetBalances(string assetHolderAddress, IEnumerable<string> contractAddresses);
        
        Task<ulong?> GetNextBlockToIndexAsync(ulong startFrom);

        Task IndexBlockAsync(ulong blockNumber, int jobVersion);
    }
}