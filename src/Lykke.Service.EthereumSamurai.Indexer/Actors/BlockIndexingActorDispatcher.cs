using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Models;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Job.EthereumSamurai.Utils;
using Lykke.Service.EthereumSamurai.Models;
using Common.Log;
using Akka.Actor;
using static Lykke.Job.EthereumSamurai.Messages.BlockIndexingActor;
using Messages = Lykke.Job.EthereumSamurai.Messages;
using static Akka.Actor.Status;
using Lykke.Job.EthereumSamurai.Actors.Factories;
using System.Collections;
using System.Collections.Generic;

namespace Lykke.Job.EthereumSamurai.Jobs
{
    public partial class BlockIndexingActorDispatcher : ReceiveActor
    {
        private readonly IBlockService _blockService;
        private readonly IIndexingService _indexingService;
        private readonly IEnumerable<IIndexingSettings> _indexingSettings;
        private readonly ILog _logger;
        private readonly IRpcBlockReader _rpcBlockReader;
        private readonly IDictionary<string, IActorRef> _blockIndexingActors;
        private readonly IDictionary<string, IIndexingSettings> _indexerSettingsDict;
        private bool _firstRun;

        public string Id => nameof(BlockIndexingJob);
        public int Version => 1;

        public BlockIndexingActorDispatcher(
            IBlockService blockService,
            IIndexingService indexingService,
            IEnumerable<IIndexingSettings> indexingSettings,
            ILog logger,
            IRpcBlockReader rpcBlockReader,
            IBlockIndexingActorFactory blockIndexingActorFactory)
        {
            _blockService = blockService;
            _firstRun = true;
            _indexingService = indexingService;
            _indexingSettings = indexingSettings;
            _logger = logger;
            _rpcBlockReader = rpcBlockReader;
            _blockIndexingActors = new Dictionary<string, IActorRef>();
            _indexerSettingsDict = new Dictionary<string, IIndexingSettings>();

            foreach (var setting in indexingSettings)
            {
                var actor = blockIndexingActorFactory.Build(Context, $"BlockIndexingActor_{setting.IndexerId}");
                _blockIndexingActors.Add(setting.IndexerId, actor);
                _indexerSettingsDict.Add(setting.IndexerId, setting);
            }

            State();

            foreach (var item in indexingSettings)
            {
                Self.Tell(Messages.Common.CreateIndexedBlockNumberMessage(item.IndexerId, 0, item.From));
            }
        }

        #region State

        public void State()
        {
            ReceiveAsync<Messages.Common.IndexedBlockNumberMessage>(async (message) =>
            {
                var nextBlock = message.NextBlock;
                IIndexingSettings settings = null;
                IActorRef blockIndexingActor = null;
                _indexerSettingsDict.TryGetValue(message.IndexerId, out settings);
                _blockIndexingActors.TryGetValue(message.IndexerId, out blockIndexingActor);

                if (settings == null || blockIndexingActor == null)
                    return;

                if (settings.To != null && nextBlock > settings.To)
                    return;

                blockIndexingActor.Tell(Messages.BlockIndexingActor.CreateIndexBlockMessage(message.IndexerId, nextBlock));
            });
        }

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
                           return Directive.Resume;
                       default:
                           return Directive.Escalate;
                   }
               });
        }

        #endregion
    }
}