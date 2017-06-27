using EthereumSamurai.Core.Models;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Indexer.Utils;
using EthereumSamurai.Models;
using EthereumSamurai.Models.Blockchain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EthereumSamurai.Indexer.Jobs
{
    public class BlockIndexingJob : IJob
    {
        private readonly IRpcBlockReader _rpcBlockReader;
        private readonly IIndexingService _indexingService;
        private readonly IIndexingSettings _indexingSettings;
        private readonly ILog _logger;
        private readonly IBlockService _blockService;
        private bool _firstRun;

        public BlockIndexingJob(IRpcBlockReader rpcBlockReader,
            IIndexingService indexingService,
            IIndexingSettings indexingSettings,
            ILog logger,
            IBlockService blockService)
        {
            _blockService = blockService;
            _logger = logger;
            _rpcBlockReader = rpcBlockReader;
            _indexingService = indexingService;
            _indexingSettings = indexingSettings;
            _firstRun = true;
        }

        public Task RunAsync()
        {
            return RunAsync(CancellationToken.None);
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(async () =>
            {
                string indexerId = _indexingSettings.IndexerId;
                BigInteger currentBlockNumber = _indexingSettings.From;
                Func<BigInteger, bool> checkDelegate = GetCheckDelegate(cancellationToken);
                BigInteger? lastSyncedNumber = await _indexingService.GetLastBlockForIndexerAsync(indexerId);
                currentBlockNumber = lastSyncedNumber.HasValue && lastSyncedNumber.Value > currentBlockNumber ? lastSyncedNumber.Value : currentBlockNumber;

                try
                {
                    if (_firstRun && _indexingSettings.From == currentBlockNumber)
                    {
                        await _logger.WriteInfoAsync("BlockIndexingJob", "RunAsync", indexerId,
                            $"Indexing begins from block-{currentBlockNumber}", DateTime.UtcNow);

                        BlockContent blockContent = await _rpcBlockReader.ReadBlockAsync(currentBlockNumber);
                        BlockContext blockContext = new BlockContext(indexerId, blockContent);

                        await _indexingService.IndexBlockAsync(blockContext);
                        currentBlockNumber++;
                        _firstRun = false;
                    }
                    //avg 400 ms per 1 block(on local machine)
                    int iterationVector = 0;
                    while (checkDelegate(currentBlockNumber))
                    {
                        await _logger.WriteInfoAsync("BlockIndexingJob", "RunAsync", indexerId,
                            $"Indexing block-{currentBlockNumber}, Vector:{iterationVector}", DateTime.UtcNow);
                        await RetryPolicy.ExecuteAsync(async () =>
                        {
                            BlockContent blockContent = await _rpcBlockReader.ReadBlockAsync(currentBlockNumber);
                            bool blockExists = await _blockService.DoesBlockExist(blockContent.BlockModel.ParentHash);
                            iterationVector = blockExists ? 1 : -1; //That is how we deal with forks
                            BlockContext blockContext = new BlockContext(indexerId, blockContent);

                            await _indexingService.IndexBlockAsync(blockContext);
                        }, 5, 100);

                        currentBlockNumber += iterationVector;
                    }
                }
                catch (Exception e)
                {
                    await _logger.WriteErrorAsync("BlockIndexingJob", "RunAsync", $"Indexing failed for block-{currentBlockNumber}", e, DateTime.UtcNow);
                    throw;
                }
                await _logger.WriteInfoAsync("BlockIndexingJob", "RunAsync", indexerId,
                    $"Indexing {indexerId} completed. LastProcessed - {currentBlockNumber - 1}", DateTime.UtcNow);

            }).Unwrap();
        }

        private Func<BigInteger, bool> GetCheckDelegate(CancellationToken cancellationToken)
        {
            Func<BigInteger, bool> checkDelegate;
            if (_indexingSettings.To == null)
            {
                checkDelegate = (number) => { return (!cancellationToken.IsCancellationRequested); };
            }
            else
            {
                checkDelegate = (number) =>
                {
                    return (!cancellationToken.IsCancellationRequested && number <= _indexingSettings.To);
                };
            }

            return checkDelegate;
        }
    }
}
