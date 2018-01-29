using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;

namespace Lykke.Service.EthereumSamurai.Core.Services
{
    public interface IInternalMessageService
    {
        Task<IEnumerable<InternalMessageModel>> GetAsync(string transactionHash);

        Task<IEnumerable<InternalMessageModel>> GetAsync(InternalMessageQuery internalMessageQuery);
    }
}