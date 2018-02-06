using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Actors.Factories
{
    public interface IChildActorFactory
    {
        IActorRef Build(IUntypedActorContext context, string name);
    }
}
