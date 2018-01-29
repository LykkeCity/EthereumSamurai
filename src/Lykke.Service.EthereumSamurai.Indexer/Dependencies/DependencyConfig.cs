using Lykke.Service.EthereumSamurai.Core.Settings;
using Lykke.Service.EthereumSamurai.Indexer.Jobs;
using Lykke.Service.EthereumSamurai.Indexer.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.EthereumSamurai.Indexer.Dependencies
{
    public static class DependencyConfig
    {
        public static IServiceCollection RegisterServices(this IServiceCollection collection, IIndexerInstanceSettings indexerInstanceSettings)
        {
            collection.AddSingleton<IBlockIndexingJobFactory,         BlockIndexingJobFactory>();
            collection.AddSingleton<IErc20BalanceIndexingJobFactory,  Erc20BalanceIndexingJobFactory>();
            collection.AddSingleton<IErc20ContractIndexingJobFactory, Erc20ContractIndexingJobFactory>();
            collection.AddSingleton<IInitalJobAssigner,               InitalJobAssigner>();

            collection.AddSingleton(indexerInstanceSettings);

            return collection;
        }
    }
}
