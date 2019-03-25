using AutoMapper;
using Common.Log;
using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Settings;
using Lykke.Service.EthereumSamurai.Filters;
using Lykke.Service.EthereumSamurai.Mappers;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;
using Lykke.Service.EthereumSamurai.MongoDb;
using Lykke.SettingsReader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;

namespace CashinReportGenerator
{
    class Program
    {
        public class UserBalanceModel
        {
            public string ClientId { get; set; }

            public string WalletBalance { get; set; }

            public string BlockchainAmount { get; set; }

            public DateTime DateTimeUtc { get; set; }
        }

        public class TransactionFeeModel
        {
            public string SenderAddress { get; set; }

            public string RecieverAddress { get; set; }

            public string EthAmount { get; set; }

            public DateTime DateTimeUtc { get; set; }

            public string TransactionHash { get; set; }

            public string FeeInEth { get; set; } //Trade, PrivateWallet
        }

        public class ExternalTransactionModel
        {
            public string ClientId { get; set; }

            public string SenderAddress { get; set; }

            public string RecieverAddress { get; set; }

            public string EthAmount { get; set; }

            public DateTime DateTimeUtc { get; set; }

            public string TransactionHash { get; set; }

            public string RecieverType { get; set; } //Trade, PrivateWallet
        }


        static void Main(string[] args)
        {
            IServiceProvider ServiceProvider;
            var settings = GetCurrentSettingsFromUrl();

            IServiceCollection collection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            collection.AddSingleton(settings);
            collection.AddSingleton<IBaseSettings>(settings.CurrentValue.EthereumIndexer);
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(typeof(Lykke.Service.EthereumSamurai.MongoDb.RegisterDependenciesExt).GetTypeInfo().Assembly);
                cfg.AddProfiles(typeof(TransactionResponseProfile).GetTypeInfo().Assembly);
            });
            collection.AddSingleton(sp => mapper.CreateMapper());
            //collection.AddSingleton<ISlackNotificationSettings>(settings.CurrentValue.SlackNotifications);
            collection.AddSingleton(settings.Nested(x => x.EthereumIndexer));
            collection.AddSingleton(settings.CurrentValue);
            var consoleLogger = new LogToConsole();
            collection.AddSingleton<ILog>(consoleLogger);
            RegisterDependenciesExt.RegisterRepositories(collection);
            ServiceProvider = collection.BuildServiceProvider();
            //Lykke.Job.EthereumCore.Config.RegisterDependency.RegisterJobs(collection);
            //var web3 = ServiceProvider.GetService<Web3>();
            //web3.Eth.GetBalance.SendRequestAsync("");
            // web3.Eth.Transactions.SendTransaction.SendRequestAsync(new Nethereum.RPC.Eth.DTOs.TransactionInput()
            //{
            //    
            //}).Result;
            //var key = EthECKey.GenerateKey().GetPrivateKeyAsBytes();
            //var stringKey = Encoding.Unicode.GetString(key);
            ServiceProvider = collection.BuildServiceProvider();

            //string coinAdapterAddress = settings.CoinAdapterAddress;
            //string balancesInfoConnString = settings.BalancesInfoConnString;
            //string bitCoinQueueConnectionString = settings.BitCoinQueueConnectionString;
            //string clientPersonalInfoConnString = settings.ClientPersonalInfoConnString;
            //string ethAssetId = settings.EthAssetId;

            //var log = ServiceProvider.GetService<ILog>();
            //var bcnRepositoryReader = new BcnClientCredentialsRepository(
            //        new AzureTableStorage<BcnCredentialsRecordEntity>(clientPersonalInfoConnString,
            //            "BcnClientCredentials", log));
            //var pendingActions = new EthererumPendingActionsRepository(
            //        new AzureTableStorage<EthererumPendingActionEntity>(bitCoinQueueConnectionString,
            //            "EthererumPendingActions", log));
            //var privateWalletsReader = new PrivateWalletsRepository(
            //        new AzureTableStorage<PrivateWalletEntity>(clientPersonalInfoConnString,
            //            "PrivateWallets", log));
            //var wallets = new WalletsRepository(new AzureTableStorage<WalletEntity>(balancesInfoConnString, "Accounts", log));
           // var assetContractService = ServiceProvider.GetService<AssetContractService>();
            var samuraiApi = ServiceProvider.GetService<ITransactionRepository>();
            var ethPrecision = BigInteger.Pow(10, 18);
            string command = "-1";
            Console.WriteLine("Type 0 - to make feesReport(Cashin) for address");
            //Console.WriteLine("Type 1 - to make feesReport(Cashout) for address");
            //Console.WriteLine("Type 1 - to make cashinReport");
            //Console.WriteLine("Type 2 - to make balance report");
            //Console.WriteLine("Type 3 - to fill pending actions for users");

            Console.WriteLine("Type exit - to quit");

            while (command != "exit")
            {
                command = Console.ReadLine();

                switch (command)
                {
                    case "0":
                    {
                        GenerateCashinReportAsync(samuraiApi);
                    }
                        break;
                    //case "3":
                    //    bcnRepositoryReader.ProcessAllAsync(async (wallet) =>
                    //    {
                    //        if (wallet.AssetId == ethAssetId)
                    //        {
                    //            BigInteger balanceOnAdapter = 0;
                    //            await RetryPolicy.ExecuteAsync(async () =>
                    //            {
                    //                balanceOnAdapter = await assetContractService.GetBalance(coinAdapterAddress, wallet.Address);
                    //                if (balanceOnAdapter > 0)
                    //                {
                    //                    await pendingActions.CreateAsync(wallet.ClientId, Guid.NewGuid().ToString());
                    //                    Console.WriteLine($"ClientId- {wallet.ClientId} added");
                    //                }
                    //            }, 3, 100);
                    //        }

                    //        Console.WriteLine($"ClientId- {wallet.ClientId} processed");
                    //    }).Wait();
                    //    break;
                    //case "1":
                    //    MakeCsvCashinReport(ethAssetId, bcnRepositoryReader, privateWalletsReader, samuraiApi);
                    //    break;
                    //case "2":
                    //    using (var streamWriter = new StreamWriter("BalancesReport"))
                    //    using (var csvWriter = new CsvHelper.CsvWriter(streamWriter, false))
                    //    {
                    //        try
                    //        {

                    //            csvWriter.WriteHeader<UserBalanceModel>();
                    //            csvWriter.NextRecord();

                    //            bcnRepositoryReader.ProcessAllAsync(async (wallet) =>
                    //        {
                    //            if (wallet.AssetId == ethAssetId)
                    //            {
                    //                double walletBalance = 0;
                    //                BigInteger balanceOnAdapter = 0;
                    //                double balanceOnAdapterCalculated = 0;
                    //                await RetryPolicy.ExecuteAsync(async () =>
                    //                {
                    //                    walletBalance = await wallets.GetWalletBalanceAsync(wallet.ClientId, ethAssetId);
                    //                    balanceOnAdapter = await assetContractService.GetBalance(coinAdapterAddress, wallet.Address);
                    //                }, 3, 100);

                    //                {
                    //                    string balanceOnAdapterString = balanceOnAdapter.ToString();
                    //                    balanceOnAdapterCalculated = (double)ConvertFromContract(balanceOnAdapterString, 18, 6);
                    //                }

                    //                if (walletBalance != balanceOnAdapterCalculated)
                    //                {
                    //                    UserBalanceModel model = new UserBalanceModel()
                    //                    {
                    //                        ClientId = wallet.ClientId,
                    //                        BlockchainAmount = balanceOnAdapterCalculated.ToString(),
                    //                        DateTimeUtc = DateTime.UtcNow,
                    //                        WalletBalance = walletBalance.ToString()
                    //                    };

                    //                    csvWriter.WriteRecord<UserBalanceModel>(model);
                    //                    csvWriter.NextRecord();
                    //                }

                    //                Console.WriteLine($"Requested {wallet.ClientId} {wallet.Address} {walletBalance} {balanceOnAdapterCalculated}");
                    //            }
                    //            else
                    //            {
                    //                Console.WriteLine($"Skipping {wallet.ClientId} {wallet.Address}");
                    //            }
                    //        }).Wait();
                    //        }
                    //        catch (Exception e)
                    //        {
                    //            Console.WriteLine($"Completly broken {e.Message} - {e.StackTrace}");
                    //        }
                    //    }

                    //    Console.WriteLine("Completed for bcn repo");
                    //    break;
                    default:
                        break;
                }
            }
        }

        private static void GenerateCashinReportAsync(ITransactionRepository samuraiApi)
        {
            string address = GetUserInputWithWalidation<string>("Type address to generate  fee report",
                "WrongAddress", (ad) => { return (true, true ? ad : "Address is not a valid deposit address"); });

            using (var streamWriter = new StreamWriter("feeReport_" + address + ".csv"))
            using (var csvWriter = new CsvHelper.CsvWriter(streamWriter, false))
            {
                Console.WriteLine("Fee report generation started!");

                int count = 1000;
                int start = 0;
                Console.WriteLine($"Asking Samurai about {address}");
                IEnumerable<TransactionModel> batchRead = null;
                csvWriter.WriteHeader<TransactionFeeModel>();
                csvWriter.NextRecord();
                var sw = new Stopwatch();

                do
                {
                    RetryPolicy.ExecuteUnlimitedAsync(async () =>
                    {
                        Console.WriteLine($"Requesting for: {address} - from :{start} - count: {count}");
                        sw.Start();
                        batchRead = await GetTransactionHistory(samuraiApi,
                            address.ToLowerInvariant(),
                            start,
                            count);
                    }, 300).Wait();
                    sw.Stop();
                    Console.WriteLine($"Time for thr request is {sw.ElapsedMilliseconds} ms");
                    sw.Reset();

                    foreach (var item in batchRead)
                    {
                        if (item.From?.ToLower() != address.ToLower())
                        {
                            continue;
                        }

                        //var amount = BigInteger.Parse(item.Value);
                        var itemFee = (item.GasPrice * item.GasUsed).ToString(CultureInfo.InvariantCulture);
                        TransactionFeeModel model = new TransactionFeeModel()
                        {
                            DateTimeUtc = UnixTimeStampToDateTime((ulong) item.BlockTimestamp),
                            EthAmount = ConvertFromContract(item.Value.ToString(), 18, 18)
                                .ToString(CultureInfo.InvariantCulture),
                            RecieverAddress = item.To,
                            SenderAddress = item.From,
                            TransactionHash = item.TransactionHash,
                            FeeInEth = ConvertFromContract(itemFee, 18, 18).ToString(CultureInfo.InvariantCulture),
                        };

                        csvWriter.WriteRecord<TransactionFeeModel>(model);
                        csvWriter.NextRecord();

                        //Console.WriteLine($"Written ${model.ToJson()}");
                    }

                    Console.WriteLine($"Completed {address} - {start} - {count}");

                    start += count;
                } while (batchRead != null && batchRead.Count() != 0);

                Console.WriteLine("Fee report generation completed!");
            }
        }

        static IReloadingManager<AppSettings> GetCurrentSettingsFromUrl()
        {
            FileInfo fi = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
            var location = Path.Combine(fi.DirectoryName);//, "..", "..", "..");
            var builder = new ConfigurationBuilder()
                .SetBasePath(location)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            var configuration = builder.Build();
            var settings = configuration.LoadSettings<AppSettings>("ConnectionStrings:ConnectionString");

            return settings;
        }

        public static async Task<IEnumerable<TransactionModel>> GetTransactionHistory(ITransactionRepository transactionRepository,
            string address, int start, int count)
        {
            var result = await transactionRepository.GetAsync(new TransactionQuery()
            {
                Count = count,
                Start = start,
                FromAddress = address
            });

            return result;
        }

        private static void ThrowOnError(object transactionResponse)
        {
            if (transactionResponse == null)
            {
                var exception = transactionResponse as ApiException;
                var errorMessage = exception?.Error?.Message ?? "Response is empty";

                throw new Exception(errorMessage);
            }
        }

        public static decimal ConvertFromContract(string amount, int multiplier, int accuracy)
        {
            if (accuracy > multiplier)
                throw new ArgumentException("accuracy > multiplier");

            multiplier -= accuracy;

            var val = BigInteger.Parse(amount);
            var res = (decimal)(val / BigInteger.Pow(10, multiplier));
            res /= (decimal)Math.Pow(10, accuracy);

            return res;
        }

        private static T GetUserInputWithWalidation<T>(string typeOfInput,
            string messageOnWrongInput,
            Func<string, (bool IsValid, T Result)> validFunc)
        {
            string input = "";

            do
            {
                Console.Write($"{typeOfInput}: ");
                input = Console.ReadLine();
                var validationResult = validFunc(input);

                if (validationResult.IsValid)
                {
                    return validationResult.Result;
                }

                Console.WriteLine($"Try Again! Error: {validationResult.Result.ToString()}");

            } while (true);
        }

        public static DateTime UnixTimeStampToDateTime(ulong unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }

    public static class Extensions
    {
        public static TConvert ConvertTo<TConvert>(this object entity) where TConvert : new()
        {
            var convertProperties = TypeDescriptor.GetProperties(typeof(TConvert)).Cast<PropertyDescriptor>();
            var entityProperties = TypeDescriptor.GetProperties(entity).Cast<PropertyDescriptor>();

            var convert = new TConvert();

            foreach (var entityProperty in entityProperties)
            {
                var property = entityProperty;
                var convertProperty = convertProperties.FirstOrDefault(prop => prop.Name == property.Name);
                if (convertProperty != null)
                {
                    convertProperty.SetValue(convert, Convert.ChangeType(entityProperty.GetValue(entity), convertProperty.PropertyType));
                }
            }

            return convert;
        }
    }

    public class SettingsWrapperExtended
    {
        public string ClientPersonalInfoConnString { get; set; }
        public string EthAssetId { get; set; }
        public string BalancesInfoConnString { get; set; }
        public string CoinAdapterAddress { get; set; }
        public string BitCoinQueueConnectionString { get; set; }
    }
}
