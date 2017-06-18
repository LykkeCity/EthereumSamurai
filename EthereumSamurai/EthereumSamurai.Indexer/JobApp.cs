using EthereumSamurai.Common;
using EthereumSamurai.Core.Models;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.Indexer.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EthereumSamurai.Indexer
{
    public class JobApp
    {
        public IServiceProvider Services { get; set; }

        public async Task Run(IConfigurationRoot configuration)
        {
            var settings = GetSettings(configuration);
            IServiceCollection collection = new ServiceCollection();
            collection.ConfigureServices(configuration);
            //RegisterJobs
            collection.AddSingleton<IBlockIndexingJobFactory, BlockIndexingJobFactory>();

            Services = collection.BuildServiceProvider();
            Console.WriteLine($"----------- Job is running now {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}-----------");

            RunJobs();
        }

        public void RunJobs()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            var factory= Services.GetService<IBlockIndexingJobFactory>();
            IEnumerable<IJob> jobs = new List<IJob>()
            {
                factory.GetJob(new IndexingSettings() { From = 1 })
            };

            JobRunner runner = new JobRunner(jobs);

            AssemblyLoadContext.Default.Unloading += ctx =>
            {
                Console.WriteLine("SIGTERM recieved");

                cts.Cancel();
            };

            runner.RunTasks(cts.Token).Wait();
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
