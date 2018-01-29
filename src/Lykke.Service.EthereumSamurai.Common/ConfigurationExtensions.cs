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
            collection.GetSettings(configuration);
            collection.ConfigureRepositories();
            collection.ConfigureServices();

            return collection;
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection collection)
        {
            collection.RegisterServices();
            return collection;
        }

        public static IServiceCollection GetSettings(
            this IServiceCollection collection,
                 IConfigurationRoot configuration)
        {
            IReloadingManager<AppSettings> settings;
            settings = configuration.LoadSettings<AppSettings>();// GeneralSettingsReader.ReadGeneralSettings<AppSettings>(connectionString);

            //IConfigurationSection indexerSettingsSection = configuration.GetSection("SettingsWrapper");
            //settings = indexerSettingsSection.Get<AppSettings>();
            
            collection.AddSingleton<IBaseSettings>(settings.CurrentValue.EthereumIndexer);
            collection.AddSingleton(settings);

            return collection;
        }
    }
}