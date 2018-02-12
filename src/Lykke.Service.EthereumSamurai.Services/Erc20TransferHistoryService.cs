using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;

namespace Lykke.Service.EthereumSamurai.Services
{
    public class Erc20TransferHistoryService : IErc20TransferHistoryService
    {
        private readonly IErc20TransferHistoryRepository _transferHistoryRepository;

        public Erc20TransferHistoryService(IErc20TransferHistoryRepository transferHistoryRepository)
        {
            _transferHistoryRepository = transferHistoryRepository;
        }

        public async Task<IEnumerable<Erc20TransferHistoryModel>> GetAsync(Erc20TransferHistoryQuery addressHistoryQuery)
        {
            return await _transferHistoryRepository.GetAsync(addressHistoryQuery);
        }
    }
}
