using Akka.Actor;
using Akka.DI.Core;
using Lykke.Job.EthereumSamurai.Actors.Factories.Implementation;
using Lykke.Job.EthereumSamurai.Jobs;
using Lykke.Service.EthereumSamurai.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Actors.Factories
{
    public class BlockIndexingActorFactory : ChildActorFactory<BlockIndexingActor>, IBlockIndexingActorFactory
    { 
        public override IActorRef Build(IUntypedActorContext context, string name)
        {
            return context.ActorOf
            (
                context.DI().Props<BlockIndexingActor>(),
                name
            );
        }
    }
}
