using AutoMapper;
using EthereumSamurai.Common;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.Indexer.Dependencies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace EthereumSamurai.IntegrationTest
{
    public static class TestConfig
    {
        private static IServiceCollection _serviceCollection;
        public static IServiceProvider ServiceProvider { get; private set; }
        public static IndexerInstanceSettings IndexerInstanceSettings { get; private set; }

        public static void ReconfigureServices ()
        {
            var url = File.ReadAllText(@"..\..\..\configurationUrl.url");
            IServiceCollection _serviceCollection = new ServiceCollection();
            var cfgBuilder = new ConfigurationBuilder();
            cfgBuilder.AddInMemoryCollection(new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("ConnectionStrings:ConnectionString", url)
            });
            IConfigurationRoot root = cfgBuilder.Build();
            #region Automapper

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(typeof(MongoDb.RegisterDependenciesExt).GetTypeInfo().Assembly);
            });
            _serviceCollection.AddSingleton(sp => mapper.CreateMapper());

            #endregion
            _serviceCollection.ConfigureServices(root);
            //IConfigurationSection indexerSettingsSection = configuration.GetSection("IndexerInstanceSettings");
            //IndexerInstanceSettings indexerSettings = indexerSettingsSection.Get<IndexerInstanceSettings>();
            IndexerInstanceSettings indexerSettings = new IndexerInstanceSettings()
            {
                IndexerId = "EthereumSamurai.IntegrationTest_0",
                StartBlock = 0,
                ThreadAmount = 1
            };
            //Register jobs and settings
            DependencyConfig.RegisterServices(_serviceCollection, indexerSettings);
            
            ServiceProvider = _serviceCollection.BuildServiceProvider();
        }
    }
}
