using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;

namespace Lykke.Service.EthereumSamurai.Core.Repositories
{
    public interface IAddressHistoryRepository
    {
        Task DeleteByHash(string hash);

        Task SaveManyForBlockAsync(IEnumerable<AddressHistoryModel> addressHistoryModels, ulong blockNumber);

        Task<IEnumerable<AddressHistoryModel>> GetAsync(AddressHistoryQuery addressHistoryQuery);
    }
}