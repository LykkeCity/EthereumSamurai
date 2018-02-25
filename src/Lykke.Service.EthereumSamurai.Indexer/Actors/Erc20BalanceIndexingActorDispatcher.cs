using System;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Services;
using Common.Log;
using Akka.Actor;
using Messages = Lykke.Job.EthereumSamurai.Messages;
using Lykke.Job.EthereumSamurai.Actors.Factories;
using Lykke.Job.EthereumSamurai.Extensions;

namespace Lykke.Job.EthereumSamurai.Jobs
{
    public partial class Erc20BalanceIndexingActorDispatcher : ReceiveActor
    {
        private readonly IErc20BalanceIndexingService _indexingService;
        private readonly ulong _startFrom;
        private readonly IActorRef _erc20BalanceIndexingActor;

        public Erc20BalanceIndexingActorDispatcher(
            IErc20BalanceIndexingService indexingService,
            IErc20BalanceIndexingActorFactory erc20BalanceIndexingActorFactory,
            ulong startFrom)
        {
            _indexingService = indexingService;
            _startFrom = startFrom;
            _erc20BalanceIndexingActor = erc20BalanceIndexingActorFactory.Build(Context, "erc20BalanceIndexingActor");

            ReceiveAsync<Messages.Common.DoIterationMessage>(async (message) =>
            {
                using (var logger = Context.GetLogger(message))
                {
                    try
                    {
                        var nextBlockToIndex = await _indexingService.GetNextBlockToIndexAsync(_startFrom);
                        if (nextBlockToIndex.HasValue)
                        {
                            await _erc20BalanceIndexingActor.Ask(
                               new Messages.Erc20BalanceIndexingActor.IndexBlockMessage(nextBlockToIndex.Value), TimeSpan.FromSeconds(90));

                            Self.Tell(new Messages.Common.DoIterationMessage());
                        }
                        else
                        {
                            Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(30), Self, new Messages.Common.DoIterationMessage(), Sender);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(2), Self, new Messages.Common.DoIterationMessage(), Sender);
                    }
                }
            });

            Self.Tell(new Messages.Common.DoIterationMessage());
        }

        public string Id
            => nameof(Erc20BalanceIndexingJob);

        public int Version
            => 1;

        #region SupervisionStrategy

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
               maxNrOfRetries: -1,
               withinTimeRange: Timeout.InfiniteTimeSpan,
               localOnlyDecider: ex =>
               {
                   switch (ex)
                   {
                       case Exception exception:
                           return Directive.Restart;
                       default:
                           return Directive.Escalate;
                   }
               });
        }

        #endregion
    }
}
