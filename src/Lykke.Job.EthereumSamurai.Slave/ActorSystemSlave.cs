using Akka.Actor;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Autofac;
using Common.Log;
using Lykke.Job.EthereumSamurai.ServiceLocation;
using Lykke.Service.EthereumSamurai.Logger;
using System.IO;
using System.Threading.Tasks;

namespace Lykke.Job.EthereumSamurai.Slave
{
    public class ActorSystemSlave
    {
        public const string ActorSystemName = "ethereumsamurai";
        private readonly ActorSystem _actorSystem;

        public ActorSystemSlave(IContainer container)
        {
            ActorLocator.Locator = container;
            //Logs are registered here
            LykkeLogger.Configure(container.Resolve<ILog>());

            var systemConfig = ConfigurationFactory.ParseString(File.ReadAllText("ethereumSamuraiSlave.hocon"));

            _actorSystem = ActorSystem.Create(ActorSystemName, systemConfig);
            var propsResolver = new AutoFacDependencyResolver(container, _actorSystem);
        }

        public void Start()
        {
        }

        public async Task StopAsync()
        {
            await _actorSystem.Terminate();
        }
    }
}
