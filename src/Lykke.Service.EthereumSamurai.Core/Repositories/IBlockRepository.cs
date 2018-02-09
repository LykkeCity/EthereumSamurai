using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models.Blockchain;

namespace Lykke.Service.EthereumSamurai.Core.Repositories
{
    public interface IBlockRepository
    {
        Task<bool> DoesBlockExistAsync(string blockHash);

        Task<BlockModel> GetForHashAsync(string blockHash);

        Task<BigInteger> GetLastSyncedBlockAsync();

        Task<BigInteger> GetSyncedBlocksCountAsync();

        Task SaveAsync(BlockModel blockModel);

        Task<IEnumerable<ulong>> GetNotSyncedBlocksNumbers(int take = 1000);

        Task PutEmptyRangeAsync(BigInteger fromBlockNumber, BigInteger toBlockNumber);
    }
}