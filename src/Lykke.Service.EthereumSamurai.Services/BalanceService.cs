using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Lykke.Service.EthereumSamurai.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly IWeb3 _web3;

        public BalanceService(IWeb3 web3)
        {
            _web3 = web3;
        }

        public async Task<BigInteger> GetBalanceAsync(string address)
        {
            BigInteger balance = await _web3.Eth.GetBalance.SendRequestAsync(address);

            return balance;
        }
    }
}
