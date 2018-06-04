using Akka.Actor;

namespace Lykke.Job.EthereumSamurai.Actors.Factories.Interfaces
{
    public interface IBlockIndexingActorFactory : IChildActorFactory
    {
        IActorRef Build(ActorSystem system, string name);

        IActorRef BuildTip(IUntypedActorContext context, string name);
    }
}
