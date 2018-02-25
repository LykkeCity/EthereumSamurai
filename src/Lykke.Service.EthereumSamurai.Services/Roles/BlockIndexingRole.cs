using Common.Log;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Core.Utils;
using Lykke.Service.EthereumSamurai.Models;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.Services.Roles.Interfaces
{
    public class BlockIndexingMiddleRole : IBlockIndexingRole
    {
        private readonly IBlockService _blockService;
        private readonly IIndexingService _indexingService;
        private readonly IRpcBlockReader _rpcBlockReader;
        private readonly IErc20TransferHistoryService _erc20TransferHistoryService;

        public string Id => nameof(BlockIndexingMiddleRole);
        public int Version => 1;

        public BlockIndexingMiddleRole(IBlockService blockService,
            IIndexingService indexingService,
            IRpcBlockReader rpcBlockReader,
            IErc20TransferHistoryService erc20TransferHistoryService)
        {
            _blockService = blockService;
            _indexingService = indexingService;
            _rpcBlockReader = rpcBlockReader;
            _erc20TransferHistoryService = erc20TransferHistoryService;
        }

        public async Task<BigInteger> IndexBlockAsync(BigInteger blockNumber)
        {
            var currentBlockNumber = blockNumber;
            int iterationVector = 0;
            BlockContent blockContent = null;
            int transactionCount = 0;

            await RetryPolicy.ExecuteAsync(async () =>
            {
                blockContent = blockContent ?? await _rpcBlockReader.ReadBlockAsync(currentBlockNumber);

                var blockContext = new BlockContext(Id, Version, blockContent);
                var blockExists = await _blockService.DoesBlockExist(blockContent.BlockModel.ParentHash);

                transactionCount = blockContent.Transactions.Count;
                iterationVector = blockExists ? 1 : -1; //That is how we deal with forks

                await _indexingService.IndexBlockAsync(blockContext);

            }, 5, 100);

            var nextBlockTooIndex = currentBlockNumber + iterationVector;

            return nextBlockTooIndex;
        }

        /*
         public async Task<BigInteger> IndexBlockAsync(BigInteger blockNumber)
        {
            var currentBlockNumber = blockNumber;
            int iterationVector = 0;
            BlockContent blockContent = null;
            int transactionCount = 0;

            await RetryPolicy.ExecuteAsync(async () =>
            {
                blockContent = blockContent ?? await _rpcBlockReader.ReadBlockAsync(currentBlockNumber);

                var blockContext = new BlockContext(Id, Version, blockContent);
                var blockExists = await _blockService.DoesBlockExist(blockContent.BlockModel.ParentHash);

                transactionCount = blockContent.Transactions.Count;
                iterationVector = blockExists ? 1 : -1; //That is how we deal with forks

                await _indexingService.IndexBlockAsync(blockContext);

            }, 5, 100);

            var nextBlockTooIndex = currentBlockNumber + iterationVector;

            return nextBlockTooIndex;
        }
         */
    }
}
