using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Common.Log;
using Lykke.Service.EthereumSamurai.Common;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Core.Settings;
using Lykke.Service.EthereumSamurai.RabbitMQ;
using Lykke.SettingsReader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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

            _services.AddSingleton<IBaseSettings>(_settings.CurrentValue.EthereumIndexer);
            _services.AddSingleton(_settings);

            IConfigurationSection indexerSettingsSection = _configuration.GetSection("IndexerInstanceSettings");
            IndexerInstanceSettings indexerSettings = indexerSettingsSection.Get<IndexerInstanceSettings>();
            _services.AddSingleton(indexerSettings);
            _services.AddSingleton<IIndexerInstanceSettings>(indexerSettings);
            _services.ConfigureServices(_configuration);
            var baseSettings = _settings.CurrentValue.EthereumIndexer;
            RegisterRabbitQueueEx.RegisterRabbitQueues(_services, baseSettings, _log);
            builder.Populate(_services);
            Console.WriteLine($"Parity node configured at {baseSettings.EthereumRpcUrl}");
        }
    }
}
