using System;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Services;
using Common.Log;
using Akka.Actor;
using Messages = Lykke.Job.EthereumSamurai.Messages;

namespace Lykke.Job.EthereumSamurai.Jobs
{
    public partial class Erc20BalanceIndexingActor : ReceiveActor
    {
        private readonly IErc20BalanceIndexingService _indexingService;
        private readonly ILog _logger;
        private readonly ulong _startFrom;

        public Erc20BalanceIndexingActor(
            IErc20BalanceIndexingService indexingService,
            ILog logger,
            ulong startFrom)
        {
            _indexingService = indexingService;
            _logger = logger;
            _startFrom = startFrom;

            ReceiveAsync<Messages.Erc20BalanceIndexingActor.IndexBlockMessage>(async (message) =>
            {
                var blockNumber = message.BlockNumber;
                await _indexingService.IndexBlockAsync(blockNumber, Version);

                await _logger.WriteInfoAsync
                (
                    nameof(Erc20BalanceIndexingJob),
                    nameof(ReceiveAsync),
                    "Block balances indexed",
                    $"Indexed balances of block {blockNumber}.",
                    DateTime.UtcNow
                );
            });
        }

        public string Id
            => nameof(Erc20BalanceIndexingJob);

        public int Version
            => 1;
    }
}