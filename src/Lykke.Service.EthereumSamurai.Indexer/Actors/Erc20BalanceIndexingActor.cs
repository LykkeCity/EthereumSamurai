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

        public Erc20BalanceIndexingActor(IErc20BalanceIndexingService indexingService)
        {
            _indexingService = indexingService;

            ReceiveAsync<Messages.Erc20BalanceIndexingActor.IndexBlockMessage>(async (message) =>
            {
                var closure = Sender;

                using (var logger = Context.GetLogger(message))
                {
                    try
                    {
                        var blockNumber = message.BlockNumber;
                        await _indexingService.IndexBlockAsync(blockNumber, Version);

                        logger.Info($"Indexed erc20 balances of block {blockNumber}");

                        closure.Tell(new Messages.Common.OperationResultMessage(true));
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);

                        closure.Tell(new Messages.Common.OperationResultMessage(true));
                    }
                }
            });
        }

        public string Id
            => nameof(Erc20BalanceIndexingActor);

        public int Version
            => 1;
    }
}
