using EthereumSamurai.Core.Settings;
using EthereumSamurai.Indexer.Jobs;
using EthereumSamurai.Indexer.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace EthereumSamurai.Indexer.Dependencies
{
    public static class DependencyConfig
    {
        public static IServiceCollection RegisterServices(this IServiceCollection collection, IIndexerInstanceSettings indexerInstanceSettings)
        {
            collection.AddSingleton<IBlockIndexingJobFactory,        BlockIndexingJobFactory>();
            collection.AddSingleton<IErc20BalanceIndexingJobFactory, Erc20BalanceIndexingJobFactory>();
            collection.AddSingleton<IInitalJobAssigner,              InitalJobAssigner>();

            collection.AddSingleton(indexerInstanceSettings);

            return collection;
        }
    }
}
