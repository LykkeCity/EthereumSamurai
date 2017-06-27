using AutoMapper;
using EthereumSamurai.Core;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;
using EthereumSamurai.MongoDb.Entities;
using EthereumSamurai.MongoDb.Utils;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.MongoDb.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IBaseSettings _baseSettings;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<TransactionEntity> _collection;
        private readonly IMapper _mapper;

        public TransactionRepository(IBaseSettings baseSettings, IMongoDatabase database, IMapper mapper)
        {
            _baseSettings = baseSettings;
            _database = database;
            _collection = database.GetCollection<TransactionEntity>(Constants.TransactionCollectionName);

            _collection.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<TransactionEntity>(Builders<TransactionEntity>.IndexKeys.Ascending(x => x.BlockNumber)),
                new CreateIndexModel<TransactionEntity>(Builders<TransactionEntity>.IndexKeys.Ascending(x => x.From)),
                new CreateIndexModel<TransactionEntity>(Builders<TransactionEntity>.IndexKeys.Ascending(x => x.To)),
                new CreateIndexModel<TransactionEntity>(Builders<TransactionEntity>.IndexKeys.Ascending(x => x.BlockTimestamp)),
                new CreateIndexModel<TransactionEntity>(Builders<TransactionEntity>.IndexKeys.Ascending(x => x.BlockHash)),
            });

            _mapper = mapper;
        }

        public async Task DeleteAllForBlockNumberAsync(ulong blockNumber)
        {
            var filter = Builders<TransactionEntity>.Filter.Eq(x => x.BlockNumber, blockNumber);

            await _collection.DeleteManyAsync(filter);
        }

        public async Task SaveAsync(TransactionModel transactionModel)
        {
            TransactionEntity transactionEntity = _mapper.Map<TransactionEntity>(transactionModel);
            await _collection.DeleteOneAsync((x) => x.TransactionHash == transactionEntity.TransactionHash);
            await _collection.InsertOneAsync(transactionEntity);
        }

        public async Task<TransactionModel> GetAsync(string transactionHash)
        {
            var filter = Builders<TransactionEntity>.Filter.Eq("_id", transactionHash);
            IAsyncCursor<TransactionEntity> cursor = await _collection.FindAsync(filter);
            TransactionEntity transactionEntity = cursor.FirstOrDefault();
            TransactionModel transactionModel = _mapper.Map<TransactionModel>(transactionEntity);

            return transactionModel;
        }

        public async Task<IEnumerable<TransactionModel>> GetAsync(TransactionQuery transactionQuery)
        {
            List<TransactionModel> result;
            var filterBuilder = Builders<TransactionEntity>.Filter;
            FilterDefinition<TransactionEntity> filter = filterBuilder.Empty;
            if (!string.IsNullOrEmpty(transactionQuery.FromAddress))
            {
                FilterDefinition<TransactionEntity> filterFrom = filterBuilder.Eq(x => x.From, transactionQuery.FromAddress);
                filter = filter & filterFrom; 
            }

            if (!string.IsNullOrEmpty(transactionQuery.ToAddress))
            {
                FilterDefinition<TransactionEntity> filterTo = filterBuilder.Eq(x => x.To, transactionQuery.ToAddress);
                filter = filter | filterTo;
            }

            if (transactionQuery.StartDate.HasValue)
            {
                int unixTime = transactionQuery.StartDate.Value.GetUnixTime();
                FilterDefinition<TransactionEntity> filterStartDate = filterBuilder.Gte(x => x.BlockTimestamp, (uint)unixTime);
                filter = filter & filterStartDate;
            }

            if (transactionQuery.EndDate.HasValue)
            {
                int unixTime = transactionQuery.EndDate.Value.GetUnixTime();
                FilterDefinition<TransactionEntity> filterEndDate = filterBuilder.Lte(x => x.BlockTimestamp, (uint)unixTime);
                filter = filter & filterEndDate;
            }

            var sort = Builders<TransactionEntity>.Sort.Ascending(x => x.BlockNumber);
            MongoDB.Driver.
            IFindFluent< TransactionEntity,TransactionEntity > search = _collection.Find(filter);
            result = new List<TransactionModel>();
            search = search.Sort(sort);
            if (transactionQuery.Start.HasValue && transactionQuery.Count.HasValue)
            {
                result = new List<TransactionModel>(transactionQuery.Count.Value - transactionQuery.Start.Value);
                search = search.Skip(transactionQuery.Start).Limit(transactionQuery.Count);
            }

            await search.ForEachAsync(transactionEntity =>
            {
                TransactionModel transactionModel = _mapper.Map<TransactionModel>(transactionEntity);
                result.Add(transactionModel);
            });

            return result;
        }

        public async Task<IEnumerable<TransactionModel>> GetForBlockNumberAsync(ulong blockNumber)
        {
            List<TransactionModel> result;
            var filterBuilder = Builders<TransactionEntity>.Filter;
            FilterDefinition<TransactionEntity> filter = filterBuilder.Eq(x => x.BlockNumber, blockNumber); ;

            IFindFluent<TransactionEntity, TransactionEntity> search = _collection.Find(filter);
            result = new List<TransactionModel>();

            await search.ForEachAsync(transactionEntity =>
            {
                TransactionModel transactionModel = _mapper.Map<TransactionModel>(transactionEntity);
                result.Add(transactionModel);
            });

            return result;
        }

        public async Task<IEnumerable<TransactionModel>> GetForBlockHashAsync(string blockHash)
        {
            List<TransactionModel> result;
            var filterBuilder = Builders<TransactionEntity>.Filter;
            FilterDefinition<TransactionEntity> filter = filterBuilder.Eq(x => x.BlockHash, blockHash); ;

            IFindFluent<TransactionEntity, TransactionEntity> search = _collection.Find(filter);
            result = new List<TransactionModel>();

            await search.ForEachAsync(transactionEntity =>
            {
                TransactionModel transactionModel = _mapper.Map<TransactionModel>(transactionEntity);
                result.Add(transactionModel);
            });

            return result;
        }
    }
}
