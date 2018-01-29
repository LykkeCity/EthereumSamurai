
using Lykke.Service.EthereumSamurai.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Repositories;

namespace Lykke.Service.EthereumSamurai.Services
{
    public class AddressHistoryService : IAddressHistoryService
    {
        private readonly IAddressHistoryRepository _addressHistoryRepository;

        public AddressHistoryService(IAddressHistoryRepository addressHistoryRepository)
        {
            _addressHistoryRepository = addressHistoryRepository;
        }

        public async Task<IEnumerable<AddressHistoryModel>> GetAsync(AddressHistoryQuery addressHistoryQuery)
        {
            IEnumerable<AddressHistoryModel> history = await _addressHistoryRepository.GetAsync(addressHistoryQuery);

            return history;
        }
    }
}
