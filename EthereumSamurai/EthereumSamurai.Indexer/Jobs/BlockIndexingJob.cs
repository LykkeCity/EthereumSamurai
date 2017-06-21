using EthereumSamurai.Core.Models;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Indexer.Utils;
using EthereumSamurai.Models;
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
        private readonly ILogger _logger;

        public BlockIndexingJob(IRpcBlockReader rpcBlockReader,
            IIndexingService indexingService,
            IIndexingSettings indexingSettings,
            ILogger logger)
        {
            _logger = logger;
            _rpcBlockReader = rpcBlockReader;
            _indexingService = indexingService;
            _indexingSettings = indexingSettings;
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
                    while (checkDelegate(currentBlockNumber))
                    {
                        _logger.LogInformation($"Indexing block-{currentBlockNumber}");
                        await RetryPolicy.ExecuteAsync(async () =>
                        {
                            BlockContent blockContent = await _rpcBlockReader.ReadBlockAsync(currentBlockNumber);
                            BlockContext blockContext = new BlockContext(indexerId, blockContent);

                            await _indexingService.IndexBlockAsync(blockContext);
                        }, 5, 100);

                        currentBlockNumber++;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(new EventId(), e, $"Indexing failed for block-{currentBlockNumber}");
                    throw;
                }

                _logger.LogInformation($"Indexing {indexerId} completed. LastProcessed - {currentBlockNumber - 1}");

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
