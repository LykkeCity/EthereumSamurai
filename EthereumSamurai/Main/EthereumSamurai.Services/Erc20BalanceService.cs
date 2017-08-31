using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;

namespace EthereumSamurai.Services
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