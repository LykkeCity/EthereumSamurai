using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Lykke.Job.EthereumSamurai.Actors.Factories.Interfaces;

namespace Lykke.Job.EthereumSamurai.Actors.Factories.Implementation
{
    public class BlockIndexingActorFactory : ChildActorFactory<BlockIndexingActor>, IBlockIndexingActorFactory
    {
        public BlockIndexingActorFactory()
        {
        }

        public IActorRef Build(ActorSystem system, string name)
        {
            return system.ActorOf
            (
                Props.Create(() => new BlockIndexingActor()).WithRouter(FromConfig.Instance),
                name
            );
        }

        public override IActorRef Build(IUntypedActorContext context, string name)
        {
            return context.ActorOf
            (
                Props.Create(() => new BlockIndexingActor()).WithRouter(FromConfig.Instance),
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
