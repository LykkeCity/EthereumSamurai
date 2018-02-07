using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Lykke.Service.EthereumSamurai.Common;
using AutoMapper;
using System.Reflection;
using Lykke.Service.EthereumSamurai.Mappers;
using Lykke.Service.EthereumSamurai.Filters;
using Lykke.Service.EthereumSamurai.Core.Services;
using Common.Log;
using Lykke.Service.EthereumSamurai.Core.Settings;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.Service.EthereumSamurai.Modules;
using Lykke.SettingsReader;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Logs;
using AzureStorage.Tables;
using Lykke.SlackNotification.AzureQueue;

namespace Lykke.Service.EthereumSamurai
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        public IContainer ApplicationContainer { get; private set; }
        public IConfigurationRoot Configuration { get; }
        public ILog Log { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Environment = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                #region Automapper

                //Add automapper
                var mapper = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfiles(typeof(MongoDb.RegisterDependenciesExt).GetTypeInfo().Assembly);
                    cfg.AddProfiles(typeof(TransactionResponseProfile).GetTypeInfo().Assembly);
                });
                services.AddSingleton(sp => mapper.CreateMapper());

                #endregion

                services.AddMvc(o =>
                {
                    o.Filters.Insert(0, new Filters.ModelStateValidationFilter());
                    o.Filters.Add(new GlobalExceptionFilter(Log));
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver =
                        new Newtonsoft.Json.Serialization.DefaultContractResolver();
                });

                services.AddSwaggerGen(options =>
                {
                    options.DefaultLykkeConfiguration("v1", "EthereumSamurai API");
                });

                var builder = new ContainerBuilder();
                var appSettings = Configuration.LoadSettings<AppSettings>();

                Log = CreateLogWithSlack(services, appSettings);

                builder.RegisterModule(new ServiceModule(appSettings, Log, Configuration));
                builder.Populate(services);
                ApplicationContainer = builder.Build();

                return new AutofacServiceProvider(ApplicationContainer);
            }
            catch (Exception ex)
            {
                Log?.WriteFatalErrorAsync(nameof(Startup), nameof(ConfigureServices), "", ex).GetAwaiter().GetResult();
                throw;
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseLykkeForwardedHeaders();
                app.UseLykkeMiddleware("EthereumSamurai", ex => new { Message = "Technical problem" });

                app.UseMvc();
                app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
                });
                app.UseSwaggerUI(x =>
                {
                    x.RoutePrefix = "swagger/ui";
                    x.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });
                app.UseStaticFiles();

                appLifetime.ApplicationStarted.Register(() => StartApplication().GetAwaiter().GetResult());
                appLifetime.ApplicationStopping.Register(() => StopApplication().GetAwaiter().GetResult());
                appLifetime.ApplicationStopped.Register(() => CleanUp().GetAwaiter().GetResult());
            }
            catch (Exception ex)
            {
                Log?.WriteFatalErrorAsync(nameof(Startup), nameof(Configure), "", ex).GetAwaiter().GetResult();
                throw;
            }
        }

        private async Task StartApplication()
        {
            try
            {
                // NOTE: Service not yet recieve and process requests here

                //await ApplicationContainer.Resolve<IStartupManager>().StartAsync();

                await Log.WriteMonitorAsync("", $"Env: {Program.EnvInfo}", "Started");
            }
            catch (Exception ex)
            {
                await Log.WriteFatalErrorAsync(nameof(Startup), nameof(StartApplication), "", ex);
                throw;
            }
        }

        private async Task StopApplication()
        {
            try
            {
                // NOTE: Service still can recieve and process requests here, so take care about it if you add logic here.

                //await ApplicationContainer.Resolve<IShutdownManager>().StopAsync();
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    await Log.WriteFatalErrorAsync(nameof(Startup), nameof(StopApplication), "", ex);
                }
                throw;
            }
        }

        private async Task CleanUp()
        {
            try
            {
                // NOTE: Service can't recieve and process requests here, so you can destroy all resources

                if (Log != null)
                {
                    await Log.WriteMonitorAsync("", $"Env: {Program.EnvInfo}", "Terminating");
                }

                ApplicationContainer.Dispose();
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    await Log.WriteFatalErrorAsync(nameof(Startup), nameof(CleanUp), "", ex);
                    (Log as IDisposable)?.Dispose();
                }
                throw;
            }
        }

        private static ILog CreateLogWithSlack(IServiceCollection services, IReloadingManager<AppSettings> settings)
        {
            var consoleLogger = new LogToConsole();
            var aggregateLogger = new AggregateLogger();

            aggregateLogger.AddLog(consoleLogger);

            //It is possible to run only with console logger
            if (!string.IsNullOrEmpty(settings.CurrentValue.EthereumIndexer.DB.LogsConnectionString)
                && !string.IsNullOrEmpty(settings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString))
            {

                var dbLogConnectionStringManager = settings.Nested(x => x.EthereumIndexer.DB.LogsConnectionString);
                var dbLogConnectionString = dbLogConnectionStringManager.CurrentValue;

                if (string.IsNullOrEmpty(dbLogConnectionString))
                {
                    consoleLogger.WriteWarningAsync(nameof(Startup), nameof(CreateLogWithSlack), "Table loggger is not inited").Wait();
                    return aggregateLogger;
                }

                if (dbLogConnectionString.StartsWith("${") && dbLogConnectionString.EndsWith("}"))
                    throw new InvalidOperationException($"LogsConnString {dbLogConnectionString} is not filled in settings");

                var persistenceManager = new LykkeLogToAzureStoragePersistenceManager(
                    AzureTableStorage<LogEntity>.Create(dbLogConnectionStringManager, "EthereumSamuraiApiLog", consoleLogger),
                    consoleLogger);

                if (settings.CurrentValue.SlackNotifications != null)
                {

                }

                // Creating slack notification service, which logs own azure queue processing messages to aggregate log
                var slackService = services.UseSlackNotificationsSenderViaAzureQueue(new AzureQueueIntegration.AzureQueueSettings
                {
                    ConnectionString = settings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString,
                    QueueName = settings.CurrentValue.SlackNotifications.AzureQueue.QueueName
                }, aggregateLogger);

                var slackNotificationsManager = new LykkeLogToAzureSlackNotificationsManager(slackService, consoleLogger);

                // Creating azure storage logger, which logs own messages to concole log
                var azureStorageLogger = new LykkeLogToAzureStorage(
                    persistenceManager,
                    slackNotificationsManager,
                    consoleLogger);

                azureStorageLogger.Start();

                aggregateLogger.AddLog(azureStorageLogger);
            }

            return aggregateLogger;
        }
    }






    //public class Startup
    //{
    //    public Startup(IHostingEnvironment env)
    //    {
    //        var builder = new ConfigurationBuilder()
    //            .SetBasePath(env.ContentRootPath)
    //            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    //            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
    //            .AddEnvironmentVariables();
    //        Configuration = builder.Build();
    //    }

    //    public IConfigurationRoot Configuration { get; }

    //    // This method gets called by the runtime. Use this method to add services to the container.
    //    public IServiceProvider ConfigureServices(IServiceCollection services)
    //    {
    //        #region Automapper

    //        //Add automapper
    //        var mapper = new MapperConfiguration(cfg =>
    //        {
    //            cfg.AddProfiles(typeof(MongoDb.RegisterDependenciesExt).GetTypeInfo().Assembly);
    //            cfg.AddProfiles(typeof(TransactionResponseProfile).GetTypeInfo().Assembly);
    //        });
    //        services.AddSingleton(sp => mapper.CreateMapper());

    //        #endregion
    //        services.ConfigureServices(Configuration);
    //        // Add framework services.
    //        var builder = services.AddMvc();
    //        var provider = services.BuildServiceProvider();
    //        builder.AddMvcOptions(o =>
    //        {
    //            o.Filters.Insert(0, new Filters.ModelStateValidationFilter());
    //            o.Filters.Add(new GlobalExceptionFilter(provider.GetService<ILog>()));
    //        });

    //        services.AddSwaggerGen(c =>
    //        {
    //            c.SingleApiVersion(new Swashbuckle.Swagger.Model.Info
    //            {
    //                Version = "v1",
    //                Title = "EthereumSamurai.Api"
    //            });
    //        });

    //        Console.WriteLine($"Geth node configured at {provider.GetService<IBaseSettings>().EthereumRpcUrl}");

    //        return services.BuildServiceProvider();
    //    }

    //    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    //    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    //    {
    //        app.UseCors((policyBuilder) =>
    //        {
    //            policyBuilder.AllowAnyHeader();
    //            policyBuilder.AllowAnyOrigin();
    //        });

    //        loggerFactory.AddConsole();
    //        loggerFactory.AddDebug();

    //        app.UseMvc();

    //        app.UseSwagger();
    //        app.UseSwaggerUi("swagger/ui/index");
    //    }
    //}
}
