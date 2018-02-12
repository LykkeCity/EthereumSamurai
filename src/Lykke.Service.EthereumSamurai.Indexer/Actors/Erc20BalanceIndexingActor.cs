using System;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Services;
using Common.Log;
using Akka.Actor;
using Messages = Lykke.Job.EthereumSamurai.Messages;
using static Akka.Actor.Status;
using Lykke.Job.EthereumSamurai.Extensions;

namespace Lykke.Job.EthereumSamurai.Jobs
{
    public partial class Erc20BalanceIndexingActor : ReceiveActor
    {
        private readonly IErc20BalanceIndexingService _indexingService;
        private readonly ulong _startFrom;

        public Erc20BalanceIndexingActor(
            IErc20BalanceIndexingService indexingService,
            ulong startFrom)
        {
            _indexingService = indexingService;
            _startFrom = startFrom;

            ReceiveAsync<Messages.Erc20BalanceIndexingActor.IndexBlockMessage>(async (message) =>
            {
                using (var logger = Context.GetLogger(message))
                {
                    var sender = Sender;
                    var blockNumber = message.BlockNumber;
                    await _indexingService.IndexBlockAsync(blockNumber, Version);

                    logger.Info($"Indexed erc20 balances of block {blockNumber}");

                    sender.Tell(new Success(true));
                }
            });
        }

        public string Id
            => nameof(Erc20BalanceIndexingJob);

        public int Version
            => 1;
    }
}