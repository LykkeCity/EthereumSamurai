using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Indexing;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Repositories
{
    public interface IBlockSyncedInfoRepository
    {
        Task SaveAsync(BlockSyncedInfoModel syncedBlockInfo);
        Task<BigInteger?> GetLastSyncedBlockForIndexerAsync(string indexerId);
        Task ClearAll();
        Task ClearForIndexer(string indexerId);
    }
}
