using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EthereumSamurai.Common;
using AutoMapper;
using System.Reflection;
using EthereumSamurai.Mappers;
using EthereumSamurai.Filters;
using EthereumSamurai.Core.Services;
using Common.Log;
using EthereumSamurai.Core.Settings;

namespace EthereumSamurai
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
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
            services.ConfigureServices(Configuration);
            // Add framework services.
            var builder = services.AddMvc();
            var provider = services.BuildServiceProvider();
            builder.AddMvcOptions(o =>
            {
                o.Filters.Insert(0, new Filters.ModelStateValidationFilter());
                o.Filters.Add(new GlobalExceptionFilter(provider.GetService<ILog>()));
            });

            services.AddSwaggerGen(c =>
            {
                c.SingleApiVersion(new Swashbuckle.Swagger.Model.Info
                {
                    Version = "v1",
                    Title = "EthereumSamurai.Api"
                });
            });

            Console.WriteLine($"Geth node configured at {provider.GetService<IBaseSettings>().EthereumRpcUrl}");

            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors((policyBuilder) =>
            {
                policyBuilder.AllowAnyHeader();
                policyBuilder.AllowAnyOrigin();
            });

            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUi("swagger/ui/index");
        }
    }
}
