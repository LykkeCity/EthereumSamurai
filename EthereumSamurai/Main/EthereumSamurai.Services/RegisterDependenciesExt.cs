using EthereumSamurai.Core.Services;
using EthereumSamurai.Core.Settings;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.Geth;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.Services
{
    public static class RegisterDependenciesExt
    {
        public static IServiceCollection RegisterServices(this IServiceCollection collection)
        {
            collection.AddSingleton<ITransactionService, TransactionService>();
            collection.AddSingleton<IIndexingService, IndexingService>();
            collection.AddSingleton<IRpcBlockReader, RpcBlockReader>();
            collection.AddSingleton<IBlockService, BlockService>();
            collection.AddSingleton<IBalanceService, BalanceService>();
            collection.AddSingleton<IDebug, DebugDecorator>();
            collection.AddSingleton<IInternalMessageService, InternalMessageService>();
            collection.AddSingleton<IAddressHistoryService, AddressHistoryService>();


            collection.AddSingleton<Web3>((provider) =>
            {
                var settings = provider.GetService<IBaseSettings>();

                return new Web3(settings.EthereumRpcUrl);
            });

            collection.AddSingleton<DebugApiService>((provider) =>
            {
                var web3 = provider.GetService<IWeb3>();

                return new DebugApiService(web3.Client);
            });

            collection.AddSingleton<IWeb3>((provider) =>
            {
                var web3 = provider.GetService<Web3>();

                return new Web3Decorator(web3);
            });

            return collection;
        }
    }
}
