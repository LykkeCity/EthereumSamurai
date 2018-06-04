using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.Util.Internal;
using Lykke.Job.EthereumSamurai.Actors.Factories;
using Lykke.Job.EthereumSamurai.Actors.Factories.Interfaces;
using Lykke.Job.EthereumSamurai.Extensions;
using Lykke.Service.EthereumSamurai.Services.Roles.Interfaces;

namespace Lykke.Job.EthereumSamurai.Actors
{
    public class BlockIndexingActorDispatcher : ReceiveActor
    {
        private readonly Guid _id = Guid.NewGuid();
        private readonly IBlockIndexingDispatcherRole _role;
        private readonly IActorRef _blockIndexingActor;
        private readonly IActorRef _tipBlockIndexingActor;
        private bool _firstRun;
        private int _needToProcessCount;
        private int _currentProcessingCount;
        private readonly IAtLeastOnceDeliveryActorFactory _atLeastOnceDeliveryActorFactory;
        private Dictionary<ulong, IActorRef> _currentWaitingActors;
        private const int BatchIndexTasks = 100;

        public BlockIndexingActorDispatcher(
            IBlockIndexingActorFactory blockIndexingActorFactory,
            IAtLeastOnceDeliveryActorFactory atLeastOnceDeliveryActorFactory,
            IBlockIndexingDispatcherRole role,
            IActorRef blockIndexerRouter)
        {
            _firstRun = true;
            _role = role;

            _blockIndexingActor = blockIndexerRouter;
            _tipBlockIndexingActor = blockIndexingActorFactory.BuildTip(Context, $"block-indexing-actor-tip");
            _atLeastOnceDeliveryActorFactory = atLeastOnceDeliveryActorFactory;

            Become(NormalState);

            Self.Tell(new Messages.Common.DoIterationMessage());
        }

        #region NormalState

        public void NormalState()
        {
            ReceiveAsync<Messages.Common.DoIterationMessage>(async (message) =>
            {
                using (var logger = Context.GetLogger(message))
                {
                    try
                    {
                        if (_firstRun)
                        {
                            logger.Info($"Start retreiving tasks");
                            Stopwatch sw = new Stopwatch();
                            sw.Start();

                            var jobInfo = await _role.RetreiveJobInfoAsync();
                            _tipBlockIndexingActor.Tell(new Messages.IndexBlockMessage(jobInfo.TipBlock));

                            logger.Info($"Retreived tasks in {sw.ElapsedMilliseconds} ms");

                            _firstRun = false;
                        }

                        var numbers = (await _role.RetreiveMiddleBlocksToIndexAsync(BatchIndexTasks))?.ToList();
                        _needToProcessCount = numbers.Count();
                        _currentProcessingCount = 0;
                        _currentWaitingActors?.ForEach(x => x.Value.Tell(PoisonPill.Instance));
                        _currentWaitingActors = new Dictionary<ulong, IActorRef>();

                        foreach (var nextBlock in numbers)
                        {
                            var atLeastOnceDeliveryActor = _atLeastOnceDeliveryActorFactory
                                .Build<Messages.IndexBlockMessage>
                                (Context, 
                                 _blockIndexingActor,
                                 Self, 
                                 $"at-least-once-delivery-proxy-{Guid.NewGuid()}");
                            _currentWaitingActors[nextBlock] = atLeastOnceDeliveryActor;

                            _blockIndexingActor.Tell(new Messages.IndexBlockMessage(nextBlock));
                        }

                        logger.Info($"Dispatched {BatchIndexTasks} more tasks at {DateTime.UtcNow} UTC ");

                        Become(BusyState);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);

                        Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(5),
                            Self, new Messages.Common.DoIterationMessage(), Self);
                    }
                }
            });

            ProcessTipActorMessages();

            ReceiveAsync<Messages.Common.IndexedBlockNumberMessage>(async (message) =>
            {
            });
        }

        #endregion

        #region BusyState

        public void BusyState()
        {
            ReceiveAsync<Messages.Common.DoIterationMessage>(async (message) =>
            {
                //Skip
            });

            ProcessTipActorMessages();

            ReceiveAsync<Messages.Common.IndexedBlockNumberMessage>(async (message) =>
            {
                _currentProcessingCount++;

                if (_currentProcessingCount == _needToProcessCount)
                {
                    Become(NormalState);
                    Self.Tell(new Messages.Common.DoIterationMessage());
                }
            });
        }

        #endregion


        #region TipActor

        private void ProcessTipActorMessages()
        {
            Receive<Messages.Common.IndexedTipBlockNumberMessage>((message) =>
            {
                _tipBlockIndexingActor.Tell(new Messages.IndexBlockMessage(message.NextBlock));
            });
        }

        #endregion


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
