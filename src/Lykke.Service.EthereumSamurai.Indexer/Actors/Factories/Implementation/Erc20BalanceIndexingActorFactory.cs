using Akka.Actor;
using Akka.DI.Core;
using Lykke.Job.EthereumSamurai.Actors.Factories.Interfaces;

namespace Lykke.Job.EthereumSamurai.Actors.Factories.Implementation
{
    public class Erc20BalanceIndexingActorFactory : ChildActorFactory<Erc20BalanceIndexingActor>, IErc20BalanceIndexingActorFactory
    {
        public override IActorRef Build(IUntypedActorContext context, string name)
        {
            return context.ActorOf
            (
                context.DI().Props<Erc20BalanceIndexingActor>(),
                name
            );
        }
    }
}
