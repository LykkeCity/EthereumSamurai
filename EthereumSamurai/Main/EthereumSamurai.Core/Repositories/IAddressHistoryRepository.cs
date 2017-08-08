using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;

namespace EthereumSamurai.Core.Repositories
{
    public interface IAddressHistoryRepository
    {
        Task DeleteByHash(string hash);

        Task SaveManyForBlockAsync(IEnumerable<AddressHistoryModel> addressHistoryModels, ulong blockNumber);

        Task<IEnumerable<AddressHistoryModel>> GetAsync(AddressHistoryQuery addressHistoryQuery);
    }
}