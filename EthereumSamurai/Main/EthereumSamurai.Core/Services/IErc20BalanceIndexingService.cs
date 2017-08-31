using System.Threading.Tasks;

namespace EthereumSamurai.Core.Services
{
    public interface IErc20BalanceIndexingService
    {
        Task<ulong?> GetNextBlockToIndexAsync(ulong startFrom);

        Task IndexBlockAsync(ulong blockNumber, int jobVersion);
    }
}