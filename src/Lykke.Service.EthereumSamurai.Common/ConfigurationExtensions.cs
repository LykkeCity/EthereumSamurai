using System;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Core.Settings;
using Lykke.Service.EthereumSamurai.MongoDb;
using Lykke.Service.EthereumSamurai.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Common.Log;
using Lykke.SettingsReader;

namespace Lykke.Service.EthereumSamurai.Common
{
    public static class Configuration
    {
        public static IServiceCollection ConfigureRepositories(this IServiceCollection collection)
        {
            collection.RegisterRepositories();

            return collection;
        }

        public static IServiceCollection ConfigureServices(
            this IServiceCollection collection,
                 IConfigurationRoot configuration)
        {
            collection.ConfigureRepositories();
            collection.ConfigureServices();

            return collection;
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection collection)
        {
            collection.RegisterServices();
            return collection;
        }
    }
}
