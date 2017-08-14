using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;

namespace EthereumSamurai.Core.Repositories
{
    public interface IErc20TransferHistoryRepository
    {
        Task<IEnumerable<Erc20TransferHistoryModel>> GetAsync(Erc20TransferHistoryQuery query);

        Task SaveForBlockAsync(IEnumerable<Erc20TransferHistoryModel> blockTransferHistory, ulong blockNumber);
        Task DeleteAllForHash(string trHash);
    }
}