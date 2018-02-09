//using System;
//using System.Numerics;
//using System.Threading;
//using System.Threading.Tasks;
//using Lykke.Service.EthereumSamurai.Core.Models;
//using Lykke.Service.EthereumSamurai.Core.Services;
//using Lykke.Service.EthereumSamurai.Models;
//using Common.Log;
//using Lykke.Service.EthereumSamurai.Core.Exceptions;
//using Lykke.Service.EthereumSamurai.Core.Utils;

//namespace Lykke.Job.EthereumSamurai.Jobs
//{
//    public class BlockIndexingJob : IJob
//    {
//        private readonly IBlockService     _blockService;
//        private readonly IIndexingService  _indexingService;
//        private readonly IIndexingSettings _indexingSettings;
//        private readonly ILog              _logger;
//        private readonly IRpcBlockReader   _rpcBlockReader;

//        private bool _firstRun;



//        public BlockIndexingJob(
//            IBlockService     blockService,
//            IIndexingService  indexingService,
//            IIndexingSettings indexingSettings,
//            ILog              logger,
//            IRpcBlockReader   rpcBlockReader)
//        {
//            _blockService     = blockService;
//            _firstRun         = true;
//            _indexingService  = indexingService;
//            _indexingSettings = indexingSettings;
//            _logger           = logger;
//            _rpcBlockReader   = rpcBlockReader;
//        }



//        public string Id 
//            => nameof(BlockIndexingJob);

//        public int Version
//            => 1;


//        public Task RunAsync()
//        {
//            return RunAsync(CancellationToken.None);
//        }

//        public Task RunAsync(CancellationToken cancellationToken)
//        {
//            return Task.Factory.StartNew(async () =>
//            {

//                var indexerId          = _indexingSettings.IndexerId;
//                var currentBlockNumber = _indexingSettings.From;
//                var checkDelegate      = GetCheckDelegate(cancellationToken);
//                var lastSyncedNumber   = await _indexingService.GetLastBlockForIndexerAsync(indexerId);

//                currentBlockNumber = lastSyncedNumber.HasValue && (lastSyncedNumber.Value > currentBlockNumber)
//                                 ? lastSyncedNumber.Value
//                                 : currentBlockNumber;

//                var lastProcessedBlockNumber = await IndexBlocksAsync(indexerId, currentBlockNumber, checkDelegate);

//                await _logger.WriteInfoAsync
//                (
//                    "BlockIndexingJob",
//                    "RunAsync",
//                    indexerId,
//                    $"Indexing {indexerId} completed. LastProcessed - {lastProcessedBlockNumber - 1}",
//                    DateTime.UtcNow
//                );

//            }, cancellationToken).Unwrap();
//        }

//        private Func<BigInteger, bool> GetCheckDelegate(CancellationToken cancellationToken)
//        {
//            Func<BigInteger, bool> checkDelegate;

//            if (_indexingSettings.To == null)
//            {
//                checkDelegate = number => !cancellationToken.IsCancellationRequested;
//            }
//            else
//            {
//                checkDelegate = number => !cancellationToken.IsCancellationRequested && number <= _indexingSettings.To;
//            }
            
//            return checkDelegate;
//        }

//        private async Task<BigInteger> IndexBlocksAsync(string indexerId, BigInteger currentBlockNumber,
//            Func<BigInteger, bool> checkDelegate)
//        {
//            try
//            {
//                if (_firstRun && _indexingSettings.From == currentBlockNumber)
//                {
//                    await _logger.WriteInfoAsync
//                    (
//                        "BlockIndexingJob",
//                        "RunAsync",
//                        indexerId,
//                        $"Indexing begins from block-{currentBlockNumber}",
//                        DateTime.UtcNow
//                    );

//                    var blockContent = await _rpcBlockReader.ReadBlockAsync(currentBlockNumber);
//                    var blockContext = new BlockContext(Id, Version, blockContent);

//                    await _indexingService.IndexBlockAsync(blockContext);

//                    currentBlockNumber++;

//                    _firstRun = false;
//                }
                
//                var iterationVector = 0;

//                while (checkDelegate(currentBlockNumber))
//                {
//                    BlockContent blockContent = null;

//                    var transactionCount = 0;

//                    await _logger.WriteInfoAsync
//                    (
//                        "BlockIndexingJob",
//                        "RunAsync",
//                        indexerId,
//                        $"Indexing block-{currentBlockNumber}, Vector:{iterationVector}",
//                        DateTime.UtcNow
//                    );

//                    await RetryPolicy.ExecuteAsync(async () =>
//                    {

//                        blockContent = blockContent ?? await _rpcBlockReader.ReadBlockAsync(currentBlockNumber);

//                        var blockContext = new BlockContext(Id, Version, blockContent);
//                        var blockExists  = await _blockService.DoesBlockExist(blockContent.BlockModel.ParentHash);

//                        transactionCount = blockContent.Transactions.Count;
//                        iterationVector  = blockExists ? 1 : -1; //That is how we deal with forks

//                        await _indexingService.IndexBlockAsync(blockContext);

//                    }, 5, 100);

//                    await _logger.WriteInfoAsync
//                    (
//                        "BlockIndexingJob",
//                        "RunAsync",
//                        indexerId,
//                        $"Indexing completed for block-{currentBlockNumber}, Vector:{iterationVector}, transaction count - {transactionCount}",
//                        DateTime.UtcNow
//                    );

//                    currentBlockNumber += iterationVector;
//                }
//            }
//            catch (Exception e)
//            {
//                if (e is BlockIsNotYetMinedException)
//                    throw;

//                await _logger.WriteErrorAsync
//                (
//                    "BlockIndexingJob",
//                    "RunAsync",
//                    $"Indexing failed for block-{currentBlockNumber}",
//                    e,
//                    DateTime.UtcNow
//                );

//                throw;
//            }

//            return currentBlockNumber;
//        }
//    }
//}