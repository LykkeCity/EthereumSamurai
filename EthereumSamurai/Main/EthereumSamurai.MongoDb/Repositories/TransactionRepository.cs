using AutoMapper;
using EthereumSamurai.Core;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.MongoDb.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
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
                new CreateIndexModel<TransactionEntity>(Builders<TransactionEntity>.IndexKeys.Ascending(x => x.BlockHash)),
            });

            _mapper = mapper;
        }

        public async Task SaveAsync(TransactionModel transactionModel)
        {
            TransactionEntity transactionEntity = _mapper.Map<TransactionEntity>(transactionModel);
            await _collection.DeleteOneAsync((x) => x.TransactionHash == transactionEntity.TransactionHash);
            await _collection.InsertOneAsync(transactionEntity);
        }
    }
}
