using System;
using System.Linq;
using Lykke.Service.EthereumSamurai.Core;
using Lykke.Service.EthereumSamurai.MongoDb.Entities;
using MongoDB.Driver;

namespace MigrationTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Connection to mongoDb should be provided");
                return;
            }

            var mongoConnString = args.First();
            var mongoClient = new MongoClient(mongoConnString);
            var database = mongoClient.GetDatabase("EthereumIndexer");

            Console.WriteLine($"Start Dropping Indicies for {Constants.Erc20TransferHistoryCollectionName}");

            var historyCollection = database.GetCollection<Erc20TransferHistoryEntity>(Constants.Erc20TransferHistoryCollectionName);

            historyCollection.Indexes.DropOne("ContractAddress_1");
            historyCollection.Indexes.DropOne("To_1_ContractAddress_1");
            historyCollection.Indexes.DropOne("From_1_ContractAddress_1");

            Console.WriteLine($"Indicies have been dropped for {Constants.Erc20TransferHistoryCollectionName}");

            Console.WriteLine($"Start Dropping Indicies for {Constants.AddressHistoryCollectionName}");

            var addressHistoryCollection = database.GetCollection<AddressHistoryEntity>(Constants.AddressHistoryCollectionName);

            addressHistoryCollection.Indexes.DropOne("From_1");
            addressHistoryCollection.Indexes.DropOne("To_1");
            addressHistoryCollection.Indexes.DropOne("BlockNumber_1");

            Console.WriteLine($"Indicies have been dropped for {Constants.Erc20TransferHistoryCollectionName}");
            Console.WriteLine("Finished");

        }
    }
}
