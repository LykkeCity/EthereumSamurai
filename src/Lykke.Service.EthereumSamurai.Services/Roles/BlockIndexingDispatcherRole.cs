using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Models.Indexing;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.Services.Roles.Interfaces
{
    public class BlockIndexingDispatcherRole : IBlockIndexingDispatcherRole
    {
        private const int _batchSize = 500;
        private const int _lastBlockMargin = 20;
        private readonly IRpcBlockReader _rpcBlockReader;
        private readonly IBlockRepository _blockRepository;

        public BlockIndexingDispatcherRole(IRpcBlockReader rpcBlockReader, IBlockRepository blockRepository)
        {
            _rpcBlockReader = rpcBlockReader;
            _blockRepository = blockRepository;
        }

        public async Task<JobInfo> RetreiveJobInfoAsync()
        {
            var block = await _blockRepository.GetLastSyncedBlockAsync();
            var lastRpcBlock = await _rpcBlockReader.GetBlockCount();
            var tip = lastRpcBlock - _lastBlockMargin;

            for (BigInteger i = block; i < tip;)
            {
                var increment = i + _batchSize;
                var to = tip < increment ? tip : increment;

                await _blockRepository.PutEmptyRangeAsync(i, to);

                i = increment;
            }

            var jobInfo = new JobInfo();
            jobInfo.TipBlock = tip;

            return jobInfo;
        }

        public async Task<IEnumerable<ulong>> RetreiveMiddleBlocksToIndex(int take = 1000)
        {
            var blocksToIndex = await _blockRepository.GetNotSyncedBlocksNumbers(take);

            return blocksToIndex;
        }
    }
}
