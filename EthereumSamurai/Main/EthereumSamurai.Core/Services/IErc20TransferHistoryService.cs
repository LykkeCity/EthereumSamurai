using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;

namespace EthereumSamurai.Core.Services
{
    public interface IErc20TransferHistoryService
    {
        Task<IEnumerable<Erc20TransferHistoryModel>> GetAsync(Erc20TransferHistoryQuery addressHistoryQuery);
    }
}