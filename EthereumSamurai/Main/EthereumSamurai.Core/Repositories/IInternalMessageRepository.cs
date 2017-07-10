using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Repositories
{
    public interface IInternalMessageRepository
    {
        Task SaveManyForBlockAsync(IEnumerable<InternalMessageModel> internalMessages, ulong blockNumber);
        Task DeleteAllForBlockNumberAsync(ulong blockNumber);
        Task<IEnumerable<InternalMessageModel>> GetAsync(string transactionHash);
        Task<IEnumerable<InternalMessageModel>> GetAllByFilterAsync(InternalMessageQuery internalMessageQuery);
    }
}
