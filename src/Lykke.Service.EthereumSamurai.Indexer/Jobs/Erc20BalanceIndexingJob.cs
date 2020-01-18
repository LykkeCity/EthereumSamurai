using System;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Services;
using Common.Log;

namespace Lykke.Job.EthereumSamurai.Jobs
{
    public class Erc20BalanceIndexingJob : IJob
    {
        private readonly IErc20BalanceIndexingService _indexingService;
        private readonly ILog                         _logger;
        private readonly ulong                        _startFrom;


        public Erc20BalanceIndexingJob(
            IErc20BalanceIndexingService indexingService,
            ILog                         logger,
            ulong                        startFrom)
        {
            _indexingService = indexingService;
            _logger          = logger;
            _startFrom       = startFrom;
        }


        public string Id
            => nameof(Erc20BalanceIndexingJob);

        public int Version
            => 1;


        public Task RunAsync()
        {
            return RunAsync(CancellationToken.None);
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    await _logger.WriteInfoAsync
                    (
                        nameof(Erc20BalanceIndexingJob),
                        nameof(RunAsync),
                        "",
                        $"Block balances indexation started.",
                        DateTime.UtcNow
                    );

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var nextBlockToIndex = await _indexingService.GetNextBlockToIndexAsync(_startFrom);

                        await _logger.WriteInfoAsync
                        (
                            nameof(Erc20BalanceIndexingJob),
                            nameof(RunAsync),
                            $"{nextBlockToIndex?.ToString() ?? "No block to index"}",
                            $"Received next block to index.",
                            DateTime.UtcNow
                        );

                        if (nextBlockToIndex.HasValue)
                        {
                            await _indexingService.IndexBlockAsync(nextBlockToIndex.Value, Version);

                            await _logger.WriteInfoAsync
                            (
                                nameof(Erc20BalanceIndexingJob),
                                nameof(RunAsync),
                                "Block balances indexed",
                                $"Indexed balances of block {nextBlockToIndex}.",
                                DateTime.UtcNow
                            );
                        }
                        else
                        {
                            await _logger.WriteInfoAsync
                            (
                                nameof(Erc20BalanceIndexingJob),
                                nameof(RunAsync),
                                "No block for balance indexation waiting for 10 sec.",
                                $"Indexed balances of block.",
                                DateTime.UtcNow
                            );
                            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                        }
                    }
                }
                catch (Exception e)
                {
                    await _logger.WriteErrorAsync
                    (
                        nameof(Erc20BalanceIndexingJob),
                        nameof(RunAsync),
                        "Balance indexing failed",
                        e,
                        DateTime.UtcNow
                    );

                    throw;
                }

                await _logger.WriteInfoAsync
                (
                    nameof(Erc20BalanceIndexingJob),
                    nameof(RunAsync),
                    "",
                    $"Block balances indexation stopped.",
                    DateTime.UtcNow
                );

            }, cancellationToken).Unwrap();
        }
    }
}