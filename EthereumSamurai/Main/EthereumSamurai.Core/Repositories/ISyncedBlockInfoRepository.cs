using System;
using System.Numerics;
using System.Threading.Tasks;
using EthereumSamurai.Models.Indexing;

namespace EthereumSamurai.Core.Repositories
{
    [Obsolete]
    public interface IBlockSyncedInfoRepository
    {
        Task ClearAll();

        Task ClearForIndexer(string indexerId);

        Task<BigInteger?> GetLastSyncedBlockForIndexerAsync(string indexerId);

        Task SaveAsync(BlockSyncedInfoModel syncedBlockInfo);
    }
}