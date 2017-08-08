using System.Numerics;
using System.Threading.Tasks;
using EthereumSamurai.Models.Blockchain;

namespace EthereumSamurai.Core.Repositories
{
    public interface IBlockRepository
    {
        Task<bool> DoesBlockExistAsync(string blockHash);

        Task<BlockModel> GetForHashAsync(string blockHash);

        Task<BigInteger> GetLastSyncedBlockAsync();

        Task<BigInteger> GetSyncedBlocksCountAsync();

        Task SaveAsync(BlockModel blockModel);
    }
}