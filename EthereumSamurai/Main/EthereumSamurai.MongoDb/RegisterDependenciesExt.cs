using AutoMapper;
using EthereumSamurai.Core.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MongoDB.Driver;
using EthereumSamurai.MongoDb.Repositories;
using EthereumSamurai.Core.Repositories;

namespace EthereumSamurai.MongoDb
{
    public static class RegisterDependenciesExt
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection collection)
        {
            #region Repositories

            try
            {
                var provider = collection.BuildServiceProvider();
                IBaseSettings settings = provider.GetService<IBaseSettings>();
                var mongoClient = new MongoClient(settings.Db.MongoDBConnectionString);

                collection.AddSingleton(typeof(MongoClient), mongoClient);
                collection.AddSingleton<IMongoDatabase>(mongoClient.GetDatabase("EthereumIndexer"));
                collection.AddSingleton<IBlockRepository, BlockRepository>();
                collection.AddSingleton<ITransactionRepository, TransactionRepository>();
                collection.AddSingleton<IBlockSyncedInfoRepository, BlockSyncedInfoRepository>();
            }
            catch (Exception e)
            {
                throw;
            }
            #endregion

            return collection;
        }
    }
}
