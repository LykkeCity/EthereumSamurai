﻿using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.MongoDb.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace EthereumSamurai.MongoDb
{
    public static class RegisterDependenciesExt
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection collection)
        {
            var provider    = collection.BuildServiceProvider();
            var settings    = provider.GetService<IBaseSettings>();
            var mongoClient = new MongoClient(settings.DB.MongoDBConnectionString);

            collection.AddSingleton(typeof(MongoClient), mongoClient);
            collection.AddSingleton(mongoClient.GetDatabase("EthereumIndexer"));

            #region Repositories

            collection.AddSingleton<IAddressHistoryRepository,       AddressHistoryRepository>();
            collection.AddSingleton<IBlockRepository,                BlockRepository>();
            collection.AddSingleton<IBlockSyncedInfoRepository,      BlockSyncedInfoRepository>();
            collection.AddSingleton<IErc20BalanceRepository,         Erc20BalanceRepository>();
            collection.AddSingleton<IErc20ContractRepository,        Erc20ContractRepository>();
            collection.AddSingleton<IErc20TransferHistoryRepository, Erc20TransferHistoryRepository>();
            collection.AddSingleton<IInternalMessageRepository,      InternalMessageRepository>();
            collection.AddSingleton<ITransactionRepository,          TransactionRepository>();

            #endregion

            return collection;
        }
    }
}