using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;

namespace Lykke.Service.EthereumSamurai.Core.Repositories
{
    public interface IErc20TransferHistoryRepository
    {
        Task<IEnumerable<Erc20TransferHistoryModel>> GetAsync(Erc20TransferHistoryQuery query);

        Task SaveForBlockAsync(IEnumerable<Erc20TransferHistoryModel> blockTransferHistory, ulong blockNumber);

        Task DeleteAllForHash(string trHash);
    }
}