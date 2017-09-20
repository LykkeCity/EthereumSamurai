using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;

namespace EthereumSamurai.Services
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