using Akka.Actor;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using Common.Log;
using Lykke.Job.EthereumSamurai.Actors.Factories;
using Lykke.Job.EthereumSamurai.Jobs;
using Lykke.Service.EthereumSamurai.Core.Models;
using Lykke.Service.EthereumSamurai.Core.Settings;
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
        private IActorRef _erc20BalanceIndexingActorDispatcher;
        private IActorRef _erc20ContractIndexingActorDispatcherProps;
        private IContainer _container;

        public ActorSystemHost()
        {
            //var systemConfig = ConfigurationFactory.FromResource
            //(
            //    "Lykke.Service.EthereumClassicApi.Actors.SystemConfig.json",
            //    typeof(ActorSystemHost).Assembly
            //);
            _actorSystem = ActorSystem.Create(actorSystemName, Config.Empty);
        }

        public void SetDependencyResolver(IContainer container)
        {
            _container = container;
            var propsResolver = new AutoFacDependencyResolver(container, _actorSystem);
        }

        public void Start(ulong currentBlockCount)
        {
            var indexerInstanceSettings = _container.Resolve<IIndexerInstanceSettings>();
            var settings = new List<IIndexingSettings>();

            // Blocks indexers
            if (indexerInstanceSettings.IndexBlocks)
            {
                var lastRpcBlock = currentBlockCount;
                var from = indexerInstanceSettings.StartBlock;
                var to = indexerInstanceSettings.StopBlock ?? lastRpcBlock;
                var partSize = (to - from) / (ulong)indexerInstanceSettings.ThreadAmount;

                ulong? toBlock = from;


                for (var i = 0; i < indexerInstanceSettings.ThreadAmount; i++)
                {
                    var fromBlock = (ulong)toBlock + 1;

                    toBlock = fromBlock + partSize;
                    toBlock = toBlock < to ? toBlock : indexerInstanceSettings.StopBlock;

                    var indexerId = $"{indexerInstanceSettings.IndexerId}_thread_{i}";
                    var setting = new IndexingSettings
                    {
                        IndexerId = indexerId,
                        From = fromBlock,
                        To = toBlock
                    };

                    settings.Add(setting);
                }

                var blockIndexingActorDispatcherProps = Props.Create(() => new BlockIndexingActorDispatcher(settings, 
                    _container.Resolve<ILog>(), _container.Resolve<IBlockIndexingActorFactory>()));
                _blockIndexingActorDispatcher = _actorSystem.ActorOf(blockIndexingActorDispatcherProps, "block-indexing-actor-dispatcher");
            }

            // Balances indexer
            if (indexerInstanceSettings.IndexBalances)
            {
                //_indexerInstanceSettings.BalancesStartBlock;
                var erc20BalanceIndexingActorDispatcherProps = _actorSystem.DI().Props<Erc20BalanceIndexingActorDispatcher>();
                _erc20BalanceIndexingActorDispatcher = _actorSystem.ActorOf(erc20BalanceIndexingActorDispatcherProps, "erc20-balance-indexing-actor-dispatcher");
            }

            // Contracts indexer
            if (indexerInstanceSettings.IndexContracts)
            {
                //indexerInstanceSettings.ContractsIndexerThreadAmount
                var erc20ContractIndexingActorDispatcherProps = _actorSystem.DI().Props<Erc20ContractIndexingActorDispatcher>();
                _erc20ContractIndexingActorDispatcherProps = _actorSystem.ActorOf(erc20ContractIndexingActorDispatcherProps, "erc20-contract-indexing-actor-dispatcher");
            }
        }

        public async Task StopAsync()
        {
            await _actorSystem.Terminate();
        }
    }
}
