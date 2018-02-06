using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.SettingsReader;
using Lykke.RabbitMqBroker.Publisher;
using AzureStorage.Blob;
using Microsoft.Extensions.DependencyInjection;
using Lykke.Service.EthereumSamurai.Core.Settings;
using Lykke.Service.EthereumSamurai.Core.Services;
using AutoMapper;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Lykke.Service.EthereumSamurai.Common;
using Lykke.Job.EthereumSamurai.Dependencies;
using Lykke.Service.EthereumSamurai.RabbitMQ;

namespace Lykke.Job.EthereumSamurai.Modules
{
    public class JobModule : Autofac.Module
    {
        private readonly IReloadingManager<AppSettings> _settings;
        private readonly ILog _log;
        private readonly IConfigurationRoot _configuration;
        private readonly IServiceCollection _services;

        public JobModule(IReloadingManager<AppSettings> appSettings, ILog log, IConfigurationRoot configuration)
        {
            _settings = appSettings;
            _log = log;
            _configuration = configuration;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<IHealthService>()
                .As<IHealthService>()
                .SingleInstance();

            #region Automapper

            //Add automapper
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(typeof(Service.EthereumSamurai.MongoDb.RegisterDependenciesExt).GetTypeInfo().Assembly);
            });
            builder.RegisterInstance(mapper.CreateMapper());

            #endregion

            IConfigurationSection indexerSettingsSection = _configuration.GetSection("IndexerInstanceSettings");
            IndexerInstanceSettings indexerSettings = indexerSettingsSection.Get<IndexerInstanceSettings>();
            _services.ConfigureServices(_configuration);
            DependencyConfig.RegisterServices(_services, indexerSettings);
            var baseSettings = _settings.CurrentValue.EthereumIndexer;
            //Console.WriteLine($"Geth node configured at {baseSettings.EthereumRpcUrl}");
            RegisterRabbitQueueEx.RegisterRabbitQueues(_services, baseSettings, _log);
            builder.Populate(_services);
        }
    }
}
