﻿using EthereumSamurai.Core.Services;
using EthereumSamurai.Core.Services.Erc20;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.Services.Erc20;
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

            #region Erc20

            collection.AddSingleton<IErc20BalanceService, Erc20BalanceService>();
            collection.AddSingleton<IErc20Detector, Erc20Detector>();
            collection.AddSingleton<IErc20TransferHistoryService, Erc20TransferHistoryService>();

            #endregion

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
