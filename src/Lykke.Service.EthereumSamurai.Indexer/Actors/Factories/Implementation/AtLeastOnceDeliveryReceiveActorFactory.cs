using Akka.Actor;
using Lykke.Job.EthereumSamurai.Actors.Delivery;
using Lykke.Job.EthereumSamurai.Messages.Delivery;
using System;
using Lykke.Job.EthereumSamurai.Actors.Factories.Interfaces;

namespace Lykke.Job.EthereumSamurai.Actors.Factories.Implementation
{
    public class AtLeastOnceDeliveryReceiveActorFactory : ChildActorFactory<AtLeastOnceDeliveryReceiveActor<Messages.IndexBlockMessage>>,
        IAtLeastOnceDeliveryActorFactory
    {
        public AtLeastOnceDeliveryReceiveActorFactory()
        {
        }

        public override IActorRef Build(IUntypedActorContext context, string name)
        {
            throw new NotImplementedException();
        }

        public IActorRef Build<T>(IUntypedActorContext context, IActorRef receiverRef, IActorRef senderRef ,string name)
            where T : IDeliverable
        {
            return context.ActorOf
            (
                Akka.Actor.Props.Create(() => new AtLeastOnceDeliveryReceiveActor<T>(receiverRef, senderRef)),
                name
            );
        }
    }
}
