using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.Core.Services
{
    public interface IErc20BalanceIndexingService
    {
        Task IndexBlockAsync(ulong blockNumber, int jobVersion);
    }
}