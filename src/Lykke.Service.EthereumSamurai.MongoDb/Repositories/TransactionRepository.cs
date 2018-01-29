using AutoMapper;
using Lykke.Service.EthereumSamurai.Core;
using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Settings;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;
using Lykke.Service.EthereumSamurai.MongoDb.Entities;
using Lykke.Service.EthereumSamurai.MongoDb.Utils;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.MongoDb.Repositories
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
                new CreateIndexModel<TransactionEntity>(Builders<TransactionEntity>.IndexKeys.Combine(
                    Builders<TransactionEntity>.IndexKeys.Ascending(x => x.From),
                    Builders<TransactionEntity>.IndexKeys.Descending(x => x.Nonce)))
            });

            _mapper = mapper;
        }

        public async Task DeleteAllForBlockNumberAsync(ulong blockNumber)
        {
            var filter = Builders<TransactionEntity>.Filter.Eq(x => x.BlockNumber, blockNumber);

            await _collection.DeleteManyAsync(filter);
        }

        public async Task DeleteByHash(string transactionHash)
        {
            var filter = Builders<TransactionEntity>.Filter.Eq(x => x.TransactionHash, transactionHash);

            await _collection.DeleteOneAsync(filter);
        }

        public async Task SaveAsync(TransactionModel transactionModel)
        {
            TransactionEntity transactionEntity = _mapper.Map<TransactionEntity>(transactionModel);
            await _collection.DeleteOneAsync((x) => x.TransactionHash == transactionEntity.TransactionHash);
            await _collection.InsertOneAsync(transactionEntity);
        }

        public async Task SaveManyForBlockAsync(IEnumerable<TransactionModel> transactionModels, ulong blockNumber)
        {
            if (transactionModels.Count() == 0)
            {
                await _collection.DeleteManyAsync((x) => x.BlockNumber == blockNumber);
                return;
            }

            var entities = new List<TransactionEntity>(transactionModels.Count());
            foreach (var transactionModel in transactionModels)
            {
                TransactionEntity transactionEntity = _mapper.Map<TransactionEntity>(transactionModel);
                entities.Add(transactionEntity);
            }

            await _collection.DeleteManyAsync((x) => x.BlockNumber == blockNumber);
            await _collection.InsertManyAsync(entities);
        }

        public async Task<TransactionModel> GetAsync(string transactionHash)
        {
            var filter            = Builders<TransactionEntity>.Filter.Eq("_id", transactionHash);
            var cursor            = await _collection.FindAsync(filter);
            var transactionEntity = cursor.FirstOrDefault();
            var transactionModel  = transactionEntity != null ? _mapper.Map<TransactionModel>(transactionEntity) : null;

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

                filter = transactionQuery.FromAddress == transactionQuery.ToAddress ? filter | filterTo : filter & filterTo;
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

            var sort = Builders<TransactionEntity>.Sort.Descending(x => x.BlockNumber);
            MongoDB.Driver.
            IFindFluent<TransactionEntity, TransactionEntity> search = _collection.Find(filter);
            result = new List<TransactionModel>();
            search = search.Sort(sort);

            transactionQuery.Start = transactionQuery.Start.HasValue ? transactionQuery.Start : 0;
            transactionQuery.Count = transactionQuery.Count.HasValue && transactionQuery.Count != 0 ? transactionQuery.Count : (int)await search.CountAsync();
            result = new List<TransactionModel>(transactionQuery.Count.Value);
            search = search.Skip(transactionQuery.Start).Limit(transactionQuery.Count);

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
