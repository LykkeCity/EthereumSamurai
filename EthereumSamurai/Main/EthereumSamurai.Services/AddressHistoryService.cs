
using EthereumSamurai.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;
using System.Threading.Tasks;
using EthereumSamurai.Core.Repositories;

namespace EthereumSamurai.Services
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
