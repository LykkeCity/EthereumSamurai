using Lykke.Service.EthereumSamurai.Core.Settings;
using Lykke.Job.EthereumSamurai.Jobs;
using Lykke.Job.EthereumSamurai.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.EthereumSamurai.Dependencies
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
