using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Lykke.Job.EthereumSamurai.Actors.Factories.Implementation;
using Lykke.Job.EthereumSamurai.Jobs;
using Lykke.Service.EthereumSamurai.Core.Models;
using Lykke.Service.EthereumSamurai.Core.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Actors.Factories
{
    public class BlockIndexingActorFactory : ChildActorFactory<BlockIndexingActor>, IBlockIndexingActorFactory
    {
        private readonly IIndexerInstanceSettings _indexerInstanceSettings;

        public BlockIndexingActorFactory(IIndexerInstanceSettings indexerInstanceSettings)
        {
            _indexerInstanceSettings = indexerInstanceSettings;
        }

        public override IActorRef Build(IUntypedActorContext context, string name)
        {
            var router = new SmallestMailboxPool(_indexerInstanceSettings.ThreadAmount);

            return context.ActorOf
            (
                context.DI().Props<BlockIndexingActor>().WithRouter(router),
                name
            );
        }

        public IActorRef BuildTip(IUntypedActorContext context, string name)
        {
            return context.ActorOf
            (
                context.DI().Props<BlockIndexingTipActor>(),
                name
            );
        }
    }
}
