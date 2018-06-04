using Akka.Actor;
using Akka.DI.Core;
using Lykke.Job.EthereumSamurai.Actors.Factories.Interfaces;

namespace Lykke.Job.EthereumSamurai.Actors.Factories.Implementation
{
    public abstract class ChildActorFactory<T> : IChildActorFactory
        where T : ActorBase
    {
        public virtual IActorRef Build(IUntypedActorContext context, string name)
        {
            return context.ActorOf
            (
                context.DI().Props<T>(),
                name
            );
        }
    }
}
