using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Repositories
{
    public interface IAddressHistoryRepository
    {
        Task DeleteByHash(string hash);
        Task SaveManyForBlockAsync(IEnumerable<AddressHistoryModel> addressHistoryModels, ulong blockNumber);
        Task<IEnumerable<AddressHistoryModel>> GetAsync(AddressHistoryQuery addressHistoryQuery);
    }
}
