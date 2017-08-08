using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;

namespace EthereumSamurai.Core.Services
{
    public interface IAddressHistoryService
    {
        Task<IEnumerable<AddressHistoryModel>> GetAsync(AddressHistoryQuery addressHistoryQuery);
    }
}