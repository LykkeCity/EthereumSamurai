using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Services
{
    public class InternalMessageService : IInternalMessageService
    {
        private readonly IInternalMessageRepository _internalMessageRepository;

        public InternalMessageService(IInternalMessageRepository internalMessageRepository)
        {
            _internalMessageRepository = internalMessageRepository;
        }

        public async Task<IEnumerable<InternalMessageModel>> GetAsync(string transactionHash)
        {
            IEnumerable<InternalMessageModel> messages = await _internalMessageRepository.GetAsync(transactionHash);

            return messages;
        }

        public async Task<IEnumerable<InternalMessageModel>> GetAsync(InternalMessageQuery internalMessageQuery)
        {
            IEnumerable<InternalMessageModel> messages = await _internalMessageRepository.GetAllByFilterAsync(internalMessageQuery);

            return messages;
        }
    }
}
