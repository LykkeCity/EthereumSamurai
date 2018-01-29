using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.Core.Repositories
{
    public interface IBlockIndexationHistoryRepository
    {
        Task<ulong?> GetLowestBlockWithNotIndexedBalances(ulong startFrom);

        Task MarkBalancesAsIndexed(ulong blockNumber, int jobVersion);

        Task MarkBlockAsIndexed(ulong blockNumber, int jobVersion);
    }
}