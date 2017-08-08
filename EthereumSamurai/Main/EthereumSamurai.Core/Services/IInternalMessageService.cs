using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;

namespace EthereumSamurai.Core.Services
{
    public interface IInternalMessageService
    {
        Task<IEnumerable<InternalMessageModel>> GetAsync(string transactionHash);

        Task<IEnumerable<InternalMessageModel>> GetAsync(InternalMessageQuery internalMessageQuery);
    }
}