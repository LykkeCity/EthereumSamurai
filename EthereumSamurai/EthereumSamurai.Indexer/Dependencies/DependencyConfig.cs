using EthereumSamurai.Core.Settings;
using EthereumSamurai.Indexer.Jobs;
using EthereumSamurai.Indexer.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.Indexer.Dependencies
{
    public static class DependencyConfig
    {
        public static IServiceCollection RegisterServices(this IServiceCollection collection, IIndexerInstanceSettings indexerInstanceSettings)
        {
            collection.AddSingleton<IBlockIndexingJobFactory, BlockIndexingJobFactory>();
            collection.AddSingleton<IIndexerInstanceSettings>(indexerInstanceSettings);
            collection.AddSingleton<IInitalJobAssigner, InitalJobAssigner>();

            return collection;
        }
    }
}
