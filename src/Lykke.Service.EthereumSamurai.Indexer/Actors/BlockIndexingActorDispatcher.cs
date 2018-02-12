using System;
using System.Numerics;
using System.Threading;
using Common.Log;
using Akka.Actor;
using Lykke.Job.EthereumSamurai.Actors.Factories;
using System.Linq;
using Lykke.Service.EthereumSamurai.Services.Roles.Interfaces;
using System.Diagnostics;
using Lykke.Service.EthereumSamurai.Logger;
using Lykke.Job.EthereumSamurai.Extensions;

namespace Lykke.Job.EthereumSamurai.Jobs
{
    public partial class BlockIndexingActorDispatcher : ReceiveActor
    {
        private readonly IBlockIndexingDispatcherRole _role;
        private readonly IActorRef _blockIndexingActor;
        private readonly IActorRef _tipBlockIndexingActor;
        private bool _firstRun;
        private int _needToProcessCount;
        private int _currentProcessingCount;
        private const int _batchIndexTasks = 1000;

        public string Id => nameof(BlockIndexingActorDispatcher);
        public int Version => 1;

        public BlockIndexingActorDispatcher(
            IBlockIndexingActorFactory blockIndexingActorFactory,
            IBlockIndexingDispatcherRole role)
        {
            _firstRun = true;
            _role = role;

            _blockIndexingActor = blockIndexingActorFactory.Build(Context, $"BlockIndexingActor");
            _tipBlockIndexingActor = blockIndexingActorFactory.BuildTip(Context, $"TipBlockIndexingActor");

            Become(NormalState);

            Self.Tell(Messages.Common.CreateDoIterationMessage());
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
                            _tipBlockIndexingActor.Tell(Messages.BlockIndexingActor.CreateIndexBlockMessage(jobInfo.TipBlock));

                            logger.Info($"Retreived tasks in {sw.ElapsedMilliseconds} ms");

                            _firstRun = false;
                        }

                        var numbers = await _role.RetreiveMiddleBlocksToIndex(_batchIndexTasks);
                        _needToProcessCount = numbers.Count();
                        _currentProcessingCount = 0;

                        foreach (var nextBlock in numbers)
                        {
                            _blockIndexingActor.Tell(Messages.BlockIndexingActor.CreateIndexBlockMessage(nextBlock));
                        }

                        Become(BusyState);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);

                        Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(5),
                            Self, Messages.Common.CreateDoIterationMessage(), Self);
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
                    Self.Tell(Messages.Common.CreateDoIterationMessage());
                }
            });
        }

        #endregion


        #region TipActor

        private void ProcessTipActorMessages()
        {
            Receive<Messages.Common.IndexedTipBlockNumberMessage>((message) =>
            {
                _tipBlockIndexingActor.Tell(new Messages.BlockIndexingActor.IndexBlockMessage(message.NextBlock));
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