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

        Task ClearForIndexer(string indexerId);

        Task<BigInteger?> GetLastSyncedBlockForIndexerAsync(string indexerId);

        Task SaveAsync(BlockSyncedInfoModel syncedBlockInfo);
    }
}