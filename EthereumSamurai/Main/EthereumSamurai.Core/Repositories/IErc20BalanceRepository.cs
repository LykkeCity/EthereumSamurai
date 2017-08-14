using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumSamurai.Models.Blockchain;

namespace EthereumSamurai.Core.Repositories
{
    public interface IErc20BalanceRepository
    {
        Task<IEnumerable<Erc20BalanceModel>> GetAsync(string assetHolderAddress, IEnumerable<string> contractAddresses);

        Task<Erc20BalanceModel> GetPreviousAsync(string assetHolderAddress, string contractAddress, ulong currentBlockNumber);

        Task SaveForBlockAsync(IEnumerable<Erc20BalanceModel> balances, ulong blockNumber);
    }
}