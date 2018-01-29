using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;

namespace Lykke.Service.EthereumSamurai.Services
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