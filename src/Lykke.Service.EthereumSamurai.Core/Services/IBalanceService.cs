using System.Numerics;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.Core.Services
{
    public interface IBalanceService
    {
        Task<BigInteger> GetBalanceAsync(string address);
    }
}