using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;

namespace EthereumSamurai.Services
{
    public class Erc20ContractService : IErc20ContractService
    {
        private readonly IErc20ContractRepository _contractRepository;

        public Erc20ContractService(
            IErc20ContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task<IEnumerable<Erc20ContractModel>> GetAsync(Erc20ContractQuery query)
        {
            return await _contractRepository.GetAsync(query);
        }
    }
}