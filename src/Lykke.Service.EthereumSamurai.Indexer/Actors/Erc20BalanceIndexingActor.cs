using System;
using Akka.Actor;
using Lykke.Job.EthereumSamurai.Extensions;
using Lykke.Service.EthereumSamurai.Core.Services;

namespace Lykke.Job.EthereumSamurai.Actors
{
    public class Erc20BalanceIndexingActor : ReceiveActor
    {
        private readonly IErc20BalanceIndexingService _indexingService;

        public Erc20BalanceIndexingActor(IErc20BalanceIndexingService indexingService)
        {
            _indexingService = indexingService;

            ReceiveAsync<Messages.IndexBlockBalancesMessage>(async (message) =>
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
