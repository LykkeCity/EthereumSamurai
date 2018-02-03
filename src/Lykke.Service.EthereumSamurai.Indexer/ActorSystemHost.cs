using Akka.Actor;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using Lykke.Job.EthereumSamurai.Jobs;
using Lykke.Service.EthereumSamurai.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.EthereumSamurai
{
    public class ActorSystemHost
    {
        public const string actorSystemName = "EthereumSamurai";
        private readonly ActorSystem _actorSystem;
        private IActorRef _blockIndexingActorDispatcher;

        public ActorSystemHost()
        {
            var systemConfig = ConfigurationFactory.FromResource
            (
                "Lykke.Service.EthereumClassicApi.Actors.SystemConfig.json",
                typeof(ActorSystemHost).Assembly
            );
            _actorSystem = ActorSystem.Create(actorSystemName, systemConfig);
        }

        public ActorSystemHost(Akka.Configuration.Config cfg)
        {
            _actorSystem = ActorSystem.Create(actorSystemName, cfg);
        }

        public void SetDependencyResolver(IContainer container)
        {
            var propsResolver = new AutoFacDependencyResolver(container, _actorSystem);
        }

        public void Start(IEnumerable<IIndexingSettings> settings)
        {
            var props = _actorSystem.DI().Props<BlockIndexingActorDispatcher>();

            _blockIndexingActorDispatcher = _actorSystem.ActorOf(props, "block-indexing-actor-dispatcher");
        }

        public async Task StopAsync()
        {
            await _actorSystem.Terminate();
        }
    }
}
