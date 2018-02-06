using Akka.Actor;
using Akka.DI.Core;
using Lykke.Job.EthereumSamurai.Actors.Factories.Implementation;
using Lykke.Job.EthereumSamurai.Jobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Actors.Factories
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
