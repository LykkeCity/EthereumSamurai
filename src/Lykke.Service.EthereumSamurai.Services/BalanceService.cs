using Lykke.Service.EthereumSamurai.Core.Services;
using Nethereum.Web3;
using System.Numerics;
using System.Threading.Tasks;

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
