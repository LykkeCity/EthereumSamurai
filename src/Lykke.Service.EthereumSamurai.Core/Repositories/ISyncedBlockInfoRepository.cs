using System;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models.Indexing;

namespace Lykke.Service.EthereumSamurai.Core.Repositories
{
    [Obsolete]
    public interface IBlockSyncedInfoRepository
    {
        Task ClearAll();

        Task<BigInteger?> GetLastSyncedBlockAsync();

        Task SaveAsync(BlockSyncedInfoModel syncedBlockInfo);
    }
}