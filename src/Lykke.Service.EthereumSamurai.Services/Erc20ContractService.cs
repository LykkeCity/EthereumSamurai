using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;

namespace Lykke.Service.EthereumSamurai.Services
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