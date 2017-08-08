using System.Numerics;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Services
{
    public interface IBalanceService
    {
        Task<BigInteger> GetBalanceAsync(string address);
    }
}