using Akka.Actor;
using Lykke.Job.EthereumSamurai.Messages.Delivery;

namespace Lykke.Job.EthereumSamurai.Actors.Factories.Interfaces
{
    public interface IAtLeastOnceDeliveryActorFactory : IChildActorFactory
    {
        IActorRef Build<T>(IUntypedActorContext context, IActorRef receiverRef, IActorRef senderRef, string name) where T : IDeliverable;
    }
}
