using AutoMapper;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.MongoDb;
using EthereumSamurai.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EthereumSamurai.Common
{
    public static class Configuration
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection collection, IConfigurationRoot configuration)
        {
            collection.GetSettings(configuration);
            
            collection.ConfigureRepositories();
            collection.ConfigureServices();
            collection.ConfigureLogging();

            return collection;
        }

        public static IServiceCollection GetSettings(this IServiceCollection collection, IConfigurationRoot configuration)
        {
            var connectionString = configuration.GetConnectionString("ConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Please, provide connection string");
            }

            var settings = GeneralSettingsReader.ReadGeneralSettings<SettingsWrapper>(connectionString);
            collection.AddSingleton<IBaseSettings>(settings.EthereumIndexer);

            return collection;
        }

        public static IServiceCollection ConfigureRepositories(this IServiceCollection collection)
        {
            collection.RegisterRepositories();
            return collection;
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection collection)
        {
            collection.RegisterServices();
            return collection;
        }

        private static IServiceCollection ConfigureLogging(this IServiceCollection collection)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole();
            collection.AddSingleton<ILoggerFactory>(loggerFactory);
            return collection;
        }

        //private static IServiceCollection ConfigureAutomapper(this IServiceCollection collection)
        //{
           
        //    return collection;
        //}


    }
}
