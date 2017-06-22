using AutoMapper;
using EthereumSamurai.Common;
using EthereumSamurai.Core.Models;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.Indexer.Jobs;
using EthereumSamurai.Indexer.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EthereumSamurai.Indexer
{
    public class JobApp
    {
        private ILoggerFactory _loggerFactory;
        private ILogger _logger;

        public IServiceProvider Services { get; set; }

        public async Task Run(IConfigurationRoot configuration)
        {
            try
            {
                //var settings = GetSettings(configuration);
                IServiceCollection collection = new ServiceCollection();
                #region Automapper

                //Add automapper
                var mapper = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfiles(typeof(MongoDb.RegisterDependenciesExt).GetTypeInfo().Assembly);
                });
                collection.AddSingleton(sp => mapper.CreateMapper());

                #endregion
                IConfigurationSection indexerSettingsSection = configuration.GetSection("IndexerInstanceSettings");
                IndexerInstanceSettings indexerSettings = indexerSettingsSection.Get<IndexerInstanceSettings>();
                collection.ConfigureServices(configuration);
                //Register jobs and settings
                collection.AddSingleton<IBlockIndexingJobFactory, BlockIndexingJobFactory>();
                collection.AddSingleton<IIndexerInstanceSettings>(indexerSettings);
                collection.AddSingleton<IInitalJobAssigner, InitalJobAssigner>();

                Services = collection.BuildServiceProvider();
                Console.WriteLine($"----------- Job is running now {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}-----------");
                _loggerFactory = Services.GetService<ILoggerFactory>();
                _logger = _loggerFactory.CreateLogger("JobApp");
                RunJobs();
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(), e, "Error on run");
                throw;
            }
        }

        public void RunJobs()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            try
            {
                
                IInitalJobAssigner initalJobAssigner = Services.GetService<IInitalJobAssigner>();
                IEnumerable<IJob> jobs = initalJobAssigner.GetJobs();

                JobRunner runner = new JobRunner(jobs, _loggerFactory.CreateLogger("JobRunner"));

                AssemblyLoadContext.Default.Unloading += ctx =>
                {
                    Console.WriteLine("SIGTERM recieved");

                    cts.Cancel();
                };

                runner.RunTasks(cts.Token).Wait();
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(), e, "Error on run jobs");
                throw;
            }
        }

        static SettingsWrapper GetSettings(IConfigurationRoot configuration)
        {
            var connectionString = configuration.GetConnectionString("ConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Please, provide connection string");
            }

            var settings = GeneralSettingsReader.ReadGeneralSettings<SettingsWrapper>(connectionString);

            return settings;
        }
    }
}
