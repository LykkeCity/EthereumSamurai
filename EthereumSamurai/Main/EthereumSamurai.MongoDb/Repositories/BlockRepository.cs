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
    public class BlockRepository : IBlockRepository
    {
        private readonly IBaseSettings _baseSettings;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<BlockEntity> _collection;
        private readonly IMapper _mapper;

        public BlockRepository(IBaseSettings baseSettings, IMongoDatabase database, IMapper mapper)
        {
            _baseSettings = baseSettings;
            _database = database;
            _collection = database.GetCollection<BlockEntity>(Constants.BlockCollectionName);

            _collection.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<BlockEntity>(Builders<BlockEntity>.IndexKeys.Ascending(x => x.Number)),
                new CreateIndexModel<BlockEntity>(Builders<BlockEntity>.IndexKeys.Descending(x => x.Timestamp)),
            });

            _mapper = mapper;
        }

        public async Task SaveAsync(BlockModel blockModel)
        {
            BlockEntity blockEntity = _mapper.Map<BlockEntity>(blockModel);
            await _collection.DeleteOneAsync((x) => x.BlockHash == blockEntity.BlockHash);
            await _collection.InsertOneAsync(blockEntity);
        }
    }
}
