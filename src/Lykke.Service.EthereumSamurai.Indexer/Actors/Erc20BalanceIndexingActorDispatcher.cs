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
    public partial class Erc20BalanceIndexingActorDispatcher : ReceiveActor
    {
        private readonly IErc20BalanceIndexingService _indexingService;
        private readonly ILog _logger;
        private readonly ulong _startFrom;
        private readonly IActorRef _erc20BalanceIndexingActor;

        public Erc20BalanceIndexingActorDispatcher(
            IErc20BalanceIndexingService indexingService,
            ILog logger,
            ulong startFrom,
            IErc20BalanceIndexingActorFactory erc20BalanceIndexingActorFactory)
        {
            _indexingService = indexingService;
            _logger = logger;
            _startFrom = startFrom;

            _erc20BalanceIndexingActor = erc20BalanceIndexingActorFactory.Build(Context, "erc20BalanceIndexingActor");

            ReceiveAsync<Messages.Common.DoIterationMessage>(async (message) =>
            {
                var nextBlockToIndex = await _indexingService.GetNextBlockToIndexAsync(_startFrom);
                if (nextBlockToIndex.HasValue)
                {
                    await _erc20BalanceIndexingActor.Ask(
                        Messages.Erc20BalanceIndexingActor.CreateIndexBlockMessage(nextBlockToIndex.Value), TimeSpan.FromSeconds(30));
                }

                Self.Tell(Messages.Common.CreateDoIterationMessage());
            });
        }

        public string Id
            => nameof(Erc20BalanceIndexingJob);

        public int Version
            => 1;
    }
}