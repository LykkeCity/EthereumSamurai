using EthereumSamurai.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Driver;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.MongoDb.Entities;
using EthereumSamurai.Core;
using System.Linq;

namespace EthereumSamurai.MongoDb.Repositories
{
    public class AddressHistoryRepository : IAddressHistoryRepository
    {
        private readonly IBaseSettings _baseSettings;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<AddressHistoryEntity> _collection;
        private readonly IMapper _mapper;

        public AddressHistoryRepository(IBaseSettings baseSettings, IMongoDatabase database, IMapper mapper)
        {
            _baseSettings = baseSettings;
            _database = database;
            _collection = database.GetCollection<AddressHistoryEntity>(Constants.AddressHistoryCollectionName);

            _collection.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<AddressHistoryEntity>(Builders<AddressHistoryEntity>.IndexKeys.Ascending(x => x.TransactionHash)),
                new CreateIndexModel<AddressHistoryEntity>(Builders<AddressHistoryEntity>.IndexKeys.Ascending(x => x.BlockNumber)),
                new CreateIndexModel<AddressHistoryEntity>(Builders<AddressHistoryEntity>.IndexKeys.Ascending(x => x.From)),
                new CreateIndexModel<AddressHistoryEntity>(Builders<AddressHistoryEntity>.IndexKeys.Ascending(x => x.To)),
                new CreateIndexModel<AddressHistoryEntity>(Builders<AddressHistoryEntity>.IndexKeys.Combine(
                    Builders<AddressHistoryEntity>.IndexKeys.Descending(x => x.BlockNumber),
                    Builders<AddressHistoryEntity>.IndexKeys.Ascending(x => x.TransactionIndex),
                    Builders<AddressHistoryEntity>.IndexKeys.Ascending(x => x.MessageIndex)))
            });

            _mapper = mapper;
        }

        public async Task DeleteByHash(string hash)
        {
            await _collection.DeleteManyAsync((x) => x.TransactionHash == hash);
        }

        public async Task<IEnumerable<AddressHistoryModel>> GetAsync(AddressHistoryQuery addressHistoryQuery)
        {
            List<AddressHistoryModel> result;
            var filterBuilder = Builders<AddressHistoryEntity>.Filter;
            FilterDefinition<AddressHistoryEntity> filter = filterBuilder.Empty;
            if (!string.IsNullOrEmpty(addressHistoryQuery.FromAddress))
            {
                FilterDefinition<AddressHistoryEntity> filterFrom = filterBuilder.Eq(x => x.From, addressHistoryQuery.FromAddress);
                filter = filter & filterFrom;
            }

            if (!string.IsNullOrEmpty(addressHistoryQuery.ToAddress))
            {
                FilterDefinition<AddressHistoryEntity> filterTo = filterBuilder.Eq(x => x.To, addressHistoryQuery.ToAddress);

                filter = addressHistoryQuery.FromAddress == addressHistoryQuery.ToAddress ? filter | filterTo : filter & filterTo;
            }

            if (addressHistoryQuery.StartBlock.HasValue)
            {
                ulong unixTime = addressHistoryQuery.StartBlock.Value;
                FilterDefinition<AddressHistoryEntity> filterStartBlock = filterBuilder.Gte(x => x.BlockNumber, unixTime);
                filter = filter & filterStartBlock;
            }

            if (addressHistoryQuery.StopBlock.HasValue)
            {
                ulong unixTime = addressHistoryQuery.StartBlock.Value;
                FilterDefinition<AddressHistoryEntity> filterEndBlock = filterBuilder.Lte(x => x.BlockNumber, unixTime);
                filter = filter & filterEndBlock;
            }

            var sort = Builders<AddressHistoryEntity>.Sort.Combine(
                 Builders<AddressHistoryEntity>.Sort.Descending(x => x.BlockNumber),
                  Builders<AddressHistoryEntity>.Sort.Ascending(x => x.TransactionIndex),
                  Builders<AddressHistoryEntity>.Sort.Ascending(x => x.MessageIndex)
                );

            MongoDB.Driver.IFindFluent<AddressHistoryEntity, AddressHistoryEntity> search = _collection.Find(filter);
            result = new List<AddressHistoryModel>();
            search = search.Sort(sort);

            addressHistoryQuery.Start = addressHistoryQuery.Start.HasValue ? addressHistoryQuery.Start : 0;
            addressHistoryQuery.Count = addressHistoryQuery.Count.HasValue && addressHistoryQuery.Count != 0 ? addressHistoryQuery.Count :
                (int)await search.CountAsync();
            result = new List<AddressHistoryModel>(addressHistoryQuery.Count.Value);
            search = search.Skip(addressHistoryQuery.Start).Limit(addressHistoryQuery.Count);

            await search.ForEachAsync(addressHistoryEntity =>
            {
                AddressHistoryModel transactionModel = _mapper.Map<AddressHistoryModel>(addressHistoryEntity);
                result.Add(transactionModel);
            });

            return result;
        }

        public async Task SaveManyForBlockAsync(IEnumerable<AddressHistoryModel> addressHistoryModels, ulong blockNumber)
        {
            if (addressHistoryModels.Count() == 0)
            {
                return;
            }

            var entities = new List<AddressHistoryEntity>(addressHistoryModels.Count());
            foreach (var addressHistory in addressHistoryModels)
            {
                AddressHistoryEntity transactionEntity = _mapper.Map<AddressHistoryEntity>(addressHistory);
                entities.Add(transactionEntity);
            }

            await _collection.DeleteManyAsync((x) => x.BlockNumber == blockNumber);
            await _collection.InsertManyAsync(entities);
        }
    }
}
