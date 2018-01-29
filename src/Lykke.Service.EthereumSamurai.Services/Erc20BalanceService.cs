using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;

namespace Lykke.Service.EthereumSamurai.Services
{
    public class Erc20BalanceService : IErc20BalanceService
    {
        private readonly IErc20BalanceRepository _balanceRepository;
        

        public Erc20BalanceService(IErc20BalanceRepository balanceRepository)
        {
            _balanceRepository = balanceRepository;
        }

        
        public async Task<IEnumerable<Erc20BalanceModel>> GetAsync(Erc20BalanceQuery query)
        {
            return await _balanceRepository.GetAsync(query);
        }
    }
}