using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;

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
            var messages = await _internalMessageRepository.GetAsync(transactionHash);

            return messages;
        }

        public async Task<IEnumerable<InternalMessageModel>> GetAsync(InternalMessageQuery internalMessageQuery)
        {
            var messages = await _internalMessageRepository.GetAllByFilterAsync(internalMessageQuery);

            return messages;
        }
    }
}