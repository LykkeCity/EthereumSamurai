using Common.Log;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Core.Utils;
using Lykke.Service.EthereumSamurai.Models;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.Services.Roles.Interfaces
{
    public class BlockIndexingRole : IBlockIndexingRole
    {
        private readonly IBlockService _blockService;
        private readonly IIndexingService _indexingService;
        private readonly ILog _logger;
        private readonly IRpcBlockReader _rpcBlockReader;

        public string Id => nameof(BlockIndexingRole);
        public int Version => 1;

        public BlockIndexingRole(IBlockService blockService,
            IIndexingService indexingService,
            ILog logger,
            IRpcBlockReader rpcBlockReader)
        {
            _blockService = blockService;
            _indexingService = indexingService;
            _logger = logger;
            _rpcBlockReader = rpcBlockReader;
        }

        public async Task<BigInteger> IndexBlockAsync(BigInteger blockNumber)
        {
            var currentBlockNumber = blockNumber;
            int iterationVector = 0;
            BlockContent blockContent = null;
            int transactionCount = 0;

            await _logger.WriteInfoAsync
            (
                "BlockIndexingJob",
                "RunAsync",
                "",
                $"Indexing block-{currentBlockNumber},",
                DateTime.UtcNow
            );

            await RetryPolicy.ExecuteAsync(async () =>
            {

                blockContent = blockContent ?? await _rpcBlockReader.ReadBlockAsync(currentBlockNumber);

                var blockContext = new BlockContext(Id, Version, blockContent);
                var blockExists = await _blockService.DoesBlockExist(blockContent.BlockModel.ParentHash);

                transactionCount = blockContent.Transactions.Count;
                iterationVector = blockExists ? 1 : -1; //That is how we deal with forks

                await _indexingService.IndexBlockAsync(blockContext);

            }, 5, 100);

            await _logger.WriteInfoAsync
            (
                "BlockIndexingJob",
                "RunAsync",
                "",
                $"Indexing completed for block-{currentBlockNumber}, Vector:{iterationVector}, transaction count - {transactionCount}",
                DateTime.UtcNow
            );

            var indexed = currentBlockNumber;
            var nextBlockTooIndex = currentBlockNumber + iterationVector;

            return nextBlockTooIndex;
        }
    }
}
