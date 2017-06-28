using EthereumSamurai.Models.Blockchain;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Repositories
{
    public interface IBlockRepository
    {
        Task<bool> DoesBlockExistAsync(string blockHash);
        Task SaveAsync(BlockModel blockModel);
        Task<BlockModel> GetForHashAsync(string blockHash);
        Task<BigInteger> GetLastSyncedBlockAsync();
        Task<BigInteger> GetSyncedBlocksCountAsync();
    }
}
