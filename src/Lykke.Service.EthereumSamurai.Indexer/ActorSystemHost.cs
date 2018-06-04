using Akka.Actor;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Autofac;
using Common.Log;
using Lykke.Job.EthereumSamurai.Actors;
using Lykke.Job.EthereumSamurai.Actors.Factories.Interfaces;
using Lykke.Job.EthereumSamurai.ServiceLocation;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Core.Settings;
using Lykke.Service.EthereumSamurai.Logger;
using Lykke.Service.EthereumSamurai.Services.Roles.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace Lykke.Job.EthereumSamurai
{
    public class ActorSystemHost
    {
        public const string ActorSystemName = "ethereumsamurai";
        private readonly ActorSystem _actorSystem;
        private IActorRef _blockIndexingActorDispatcher;
        private IActorRef _erc20BalanceIndexingActorDispatcher;
        private IActorRef _erc20ContractIndexingActorDispatcherProps;
        private IContainer _container;

        public ActorSystemHost(IContainer container)
        {
            ActorLocator.Locator = container;
            _container = container;
            LykkeLogger.Configure(container.Resolve<ILog>());
            //Logs are registered here

            var systemConfig = ConfigurationFactory.ParseString(File.ReadAllText("ethereumSamurai.hocon"));
            _actorSystem = ActorSystem.Create(ActorSystemName, systemConfig);
            var propsResolver = new AutoFacDependencyResolver(container, _actorSystem);
        }

        public void Start()
        {
            var indexerInstanceSettings = _container.Resolve<IIndexerInstanceSettings>();

            // Blocks indexers
            if (indexerInstanceSettings.IndexBlocks)
            {
                var blockIndexerRouter = _container.Resolve<IBlockIndexingActorFactory>().Build(_actorSystem, "block-indexing-actor");
                var blockIndexingActorDispatcherProps = Props.Create(() => new BlockIndexingActorDispatcher(
                    _container.Resolve<IBlockIndexingActorFactory>(),
                    _container.Resolve<IAtLeastOnceDeliveryActorFactory>(),
                    _container.Resolve<IBlockIndexingDispatcherRole>(),
                    blockIndexerRouter));
                var clusterSingletonProps = ClusterSingletonManager.Props(
                    singletonProps: blockIndexingActorDispatcherProps,
                    terminationMessage: PoisonPill.Instance,
                    settings: ClusterSingletonManagerSettings.Create(_actorSystem).WithRole("indexer"));

                _blockIndexingActorDispatcher = _actorSystem.ActorOf(clusterSingletonProps, "block-indexing-actor-dispatcher");
            }

            // Balances indexer
            if (indexerInstanceSettings.IndexBalances)
            {
                var erc20BalanceIndexingActorDispatcherProps = Props.Create(() => new Erc20BalanceIndexingActorDispatcher(
                    _container.Resolve<IErc20BalanceIndexingService>(),
                     _container.Resolve<IErc20BalanceIndexingActorFactory>(),
                     indexerInstanceSettings.BalancesStartBlock));

                var clusterSingletonProps = ClusterSingletonManager.Props(
                    singletonProps: erc20BalanceIndexingActorDispatcherProps,
                    terminationMessage: PoisonPill.Instance,
                    settings: ClusterSingletonManagerSettings.Create(_actorSystem).WithRole("indexer"));

                _erc20BalanceIndexingActorDispatcher = _actorSystem.ActorOf(clusterSingletonProps, "erc20-balance-indexing-actor-dispatcher");
            }

            // Contracts indexer
            if (indexerInstanceSettings.IndexContracts)
            {
                var erc20ContractIndexingActorDispatcherProps = Props.Create(() => new Erc20ContractIndexingActorDispatcher(
                    _container.Resolve<IErc20ContractIndexingService>(),
                    _container.Resolve<ILog>(),
                    _container.Resolve<IErc20ContractIndexingActorFactory>()));

                var clusterSingletonProps = ClusterSingletonManager.Props(
                    singletonProps: erc20ContractIndexingActorDispatcherProps,
                    terminationMessage: PoisonPill.Instance,
                    settings: ClusterSingletonManagerSettings.Create(_actorSystem).WithRole("indexer"));

                _erc20ContractIndexingActorDispatcherProps = _actorSystem.ActorOf(erc20ContractIndexingActorDispatcherProps, "erc20-contract-indexing-actor-dispatcher");
            }
        }

        public async Task StopAsync()
        {
            await _actorSystem.Terminate();
        }
    }
}
