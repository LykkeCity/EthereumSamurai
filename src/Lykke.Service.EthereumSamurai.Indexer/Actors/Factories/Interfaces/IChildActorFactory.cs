using Akka.Actor;

namespace Lykke.Job.EthereumSamurai.Actors.Factories.Interfaces
{
    public interface IChildActorFactory
    {
        IActorRef Build(IUntypedActorContext context, string name);
    }
}
