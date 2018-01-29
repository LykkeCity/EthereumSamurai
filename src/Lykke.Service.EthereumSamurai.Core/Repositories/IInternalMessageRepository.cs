using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;

namespace Lykke.Service.EthereumSamurai.Core.Repositories
{
    public interface IInternalMessageRepository
    {
        Task DeleteAllForBlockNumberAsync(ulong blockNumber);

        Task DeleteAllForHash(string transactionHash);

        Task<IEnumerable<InternalMessageModel>> GetAllByFilterAsync(InternalMessageQuery internalMessageQuery);

        Task<IEnumerable<InternalMessageModel>> GetAsync(string transactionHash);

        Task SaveManyForBlockAsync(IEnumerable<InternalMessageModel> internalMessages, ulong blockNumber);
    }
}