using System;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Services;
using Common.Log;
using Akka.Actor;
using Messages = Lykke.Job.EthereumSamurai.Messages;
using Lykke.Job.EthereumSamurai.Actors.Factories;

namespace Lykke.Job.EthereumSamurai.Jobs
{
    public partial class Erc20ContractIndexingActorDispatcher : ReceiveActor
    {
        private readonly IErc20ContractIndexingService _indexingService;
        private readonly ILog _logger;
        private readonly ulong _startFrom;
        private readonly IActorRef _erc20ContractIndexingActor;

        public Erc20ContractIndexingActorDispatcher(
            IErc20ContractIndexingService indexingService,
            ILog logger,
            ulong startFrom,
            IErc20ContractIndexingActorFactory erc20ContractIndexingActorFactory)
        {
            _indexingService = indexingService;
            _logger = logger;
            _startFrom = startFrom;

            _erc20ContractIndexingActor = erc20ContractIndexingActorFactory.Build(Context, "erc20ContractIndexingActor");

            ReceiveAsync<Messages.Common.DoIterationMessage>(async (message) =>
            {
                var model = await _indexingService.GetNextContractToIndexAsync();
                if (model != null)
                {
                    await _erc20ContractIndexingActor.Ask(
                        new Messages.Erc20ContractIndexingActor.Erc20ContractDeployedMessage(model), TimeSpan.FromSeconds(30));

                    Self.Tell(Messages.Common.CreateDoIterationMessage());
                }
                else
                {
                    Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(10), Self, Messages.Common.CreateDoIterationMessage(), Self);
                }
            });
        }

        public string Id
            => nameof(Erc20BalanceIndexingJob);

        public int Version
            => 1;
    }
}