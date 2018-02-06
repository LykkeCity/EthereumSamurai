using Akka.Actor;
using Lykke.Service.EthereumSamurai.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Actors.Factories
{
    public interface IBlockIndexingActorFactory : IChildActorFactory
    {
    }
}
