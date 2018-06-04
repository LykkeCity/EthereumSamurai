using System;
using Akka.Actor;
using Common.Log;
using Lykke.Job.EthereumSamurai.Actors.Factories;
using Lykke.Job.EthereumSamurai.Actors.Factories.Interfaces;
using Lykke.Job.EthereumSamurai.Messages;
using Lykke.Service.EthereumSamurai.Core.Services;

namespace Lykke.Job.EthereumSamurai.Actors
{
    public class Erc20ContractIndexingActorDispatcher : ReceiveActor
    {
        private readonly IErc20ContractIndexingService _indexingService;
        private readonly ILog _logger;
        private readonly IActorRef _erc20ContractIndexingActor;

        public Erc20ContractIndexingActorDispatcher(
            IErc20ContractIndexingService indexingService,
            ILog logger,
            IErc20ContractIndexingActorFactory erc20ContractIndexingActorFactory)
        {
            _indexingService = indexingService;
            _logger = logger;
            _erc20ContractIndexingActor = erc20ContractIndexingActorFactory.Build(Context, "erc20ContractIndexingActor");

            ReceiveAsync<Messages.Common.DoIterationMessage>(async (message) =>
            {
                var model = await _indexingService.GetNextContractToIndexAsync();
                if (model != null)
                {
                    await _erc20ContractIndexingActor.Ask(
                        new Erc20ContractDeployedMessage(model), TimeSpan.FromSeconds(30));

                    Self.Tell(new Messages.Common.DoIterationMessage());
                }
                else
                {
                    Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(10), Self, new Messages.Common.DoIterationMessage(), Self);
                }
            });
        }
    }
}
