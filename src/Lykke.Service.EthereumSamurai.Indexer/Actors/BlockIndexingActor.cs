using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Models;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Job.EthereumSamurai.Utils;
using Lykke.Service.EthereumSamurai.Models;
using Common.Log;
using Akka.Actor;
using Lykke.Job.EthereumSamurai.Messages;
using static Lykke.Job.EthereumSamurai.Messages.BlockIndexingActor;
using Messages = Lykke.Job.EthereumSamurai.Messages;
using static Akka.Actor.Status;

namespace Lykke.Job.EthereumSamurai.Jobs
{
    public partial class BlockIndexingActor : ReceiveActor
    {
        private readonly IBlockService _blockService;
        private readonly IIndexingService _indexingService;
        private readonly IIndexingSettings _indexingSettings;
        private readonly ILog _logger;
        private readonly IRpcBlockReader _rpcBlockReader;

        private bool _firstRun;

        public string Id => nameof(BlockIndexingJob);
        public int Version => 1;

        public BlockIndexingActor(
            IBlockService blockService,
            IIndexingService indexingService,
            IIndexingSettings indexingSettings,
            ILog logger,
            IRpcBlockReader rpcBlockReader)
        {
            _blockService = blockService;
            _firstRun = true;
            _indexingService = indexingService;
            _indexingSettings = indexingSettings;
            _logger = logger;
            _rpcBlockReader = rpcBlockReader;
        }

        #region FirstRun

        public void FirstRun()
        {
            ReceiveAsync<IndexBlockMessage>(async (message) =>
            {
                var indexerId = message.IndexerId;
                var currentBlockNumber = message.BlockNumber;

                if (_firstRun && _indexingSettings.From == currentBlockNumber)
                {
                    await _logger.WriteInfoAsync
                    (
                        "BlockIndexingJob",
                        "RunAsync",
                        indexerId,
                        $"Indexing begins from block-{currentBlockNumber}",
                        DateTime.UtcNow
                    );

                    var blockContent = await _rpcBlockReader.ReadBlockAsync(currentBlockNumber);
                    var blockContext = new BlockContext(Id, Version, indexerId, blockContent);

                    await _indexingService.IndexBlockAsync(blockContext);

                    currentBlockNumber++;

                    _firstRun = false;

                    //After first run
                    Sender.Tell(Messages.Common.CreateIndexedBlockNumberMessage(indexerId, message.BlockNumber, currentBlockNumber));

                    Become(NormalState);
                }
            });
        }

        #endregion

        #region NormalState

        public void NormalState()
        {
            ReceiveAsync<IndexBlockMessage>(async (message) =>
            {
                var indexerId = message.IndexerId;
                var currentBlockNumber = message.BlockNumber;
                int iterationVector = 0;
                BlockContent blockContent = null;
                int transactionCount = 0;

                await _logger.WriteInfoAsync
                (
                    "BlockIndexingJob",
                    "RunAsync",
                    indexerId,
                    $"Indexing block-{currentBlockNumber},",
                    DateTime.UtcNow
                );

                await RetryPolicy.ExecuteAsync(async () =>
                {

                    blockContent = blockContent ?? await _rpcBlockReader.ReadBlockAsync(currentBlockNumber);

                    var blockContext = new BlockContext(Id, Version, indexerId, blockContent);
                    var blockExists = await _blockService.DoesBlockExist(blockContent.BlockModel.ParentHash);

                    transactionCount = blockContent.Transactions.Count;
                    iterationVector = blockExists ? 1 : -1; //That is how we deal with forks

                    await _indexingService.IndexBlockAsync(blockContext);

                }, 5, 100);

                await _logger.WriteInfoAsync
                (
                    "BlockIndexingJob",
                    "RunAsync",
                    indexerId,
                    $"Indexing completed for block-{currentBlockNumber}, Vector:{iterationVector}, transaction count - {transactionCount}",
                    DateTime.UtcNow
                );

                var indexed = currentBlockNumber;
                currentBlockNumber += iterationVector;

                Sender.Tell(Messages.Common.CreateIndexedBlockNumberMessage(indexerId, indexed, currentBlockNumber));
            });
        }

        #endregion
    }
}