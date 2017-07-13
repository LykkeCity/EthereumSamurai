using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Services
{
    public interface IInternalMessageService
    {
        Task<IEnumerable<InternalMessageModel>> GetAsync(string transactionHash);
        Task<IEnumerable<InternalMessageModel>> GetAsync(InternalMessageQuery internalMessageQuery);
    }
}
