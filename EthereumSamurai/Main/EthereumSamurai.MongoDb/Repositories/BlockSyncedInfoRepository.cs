using AutoMapper;
using EthereumSamurai.Core;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Indexing;
using EthereumSamurai.MongoDb.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.MongoDb.Repositories
{
    public class BlockSyncedInfoRepository : IBlockSyncedInfoRepository
    {
        private readonly IBaseSettings _baseSettings;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<BlockSyncedInfoEntity> _collection;
        private readonly IMapper _mapper;

        public BlockSyncedInfoRepository(IBaseSettings baseSettings, IMongoDatabase database, IMapper mapper)
        {
            _baseSettings = baseSettings;
            _database = database;
            _collection = database.GetCollection<BlockSyncedInfoEntity>(Constants.BlockSyncedInfoCollectionName);

            _collection.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<BlockSyncedInfoEntity>(Builders<BlockSyncedInfoEntity>.IndexKeys.Ascending(x => x.IndexerId))
            });

            _mapper = mapper;
        }

        public async Task SaveAsync(BlockSyncedInfoModel syncedBlockInfo)
        {
            BlockSyncedInfoEntity blockSyncedInfoEntity = _mapper.Map<BlockSyncedInfoEntity>(syncedBlockInfo);

            await _collection.DeleteOneAsync((x) => x.BlockNumber == blockSyncedInfoEntity.BlockNumber);
            await _collection.InsertOneAsync(blockSyncedInfoEntity);
        }

        public async Task<BigInteger?> GetLastSyncedBlockForIndexerAsync(string indexerId)
        {
            var filterBuilder = Builders<BlockSyncedInfoEntity>.Filter;
            FilterDefinition<BlockSyncedInfoEntity> filter = filterBuilder.Eq(x => x.IndexerId, indexerId);
            var sort = Builders<BlockSyncedInfoEntity>.Sort.Descending(x => x.BlockNumber);
            BlockSyncedInfoEntity result = await _collection.Find<BlockSyncedInfoEntity>(filter).Sort(sort).FirstOrDefaultAsync();

            return result != null ? new BigInteger?(result.BlockNumber) : null;
        }

        public async Task ClearAll()
        {
            var filterBuilder = Builders<BlockSyncedInfoEntity>.Filter;
            FilterDefinition<BlockSyncedInfoEntity> filter = filterBuilder.Empty;

            await _collection.DeleteManyAsync(filter);
        }

        public async Task ClearForIndexer(string indexerId)
        {
            var filterBuilder = Builders<BlockSyncedInfoEntity>.Filter;
            FilterDefinition<BlockSyncedInfoEntity> filter = filterBuilder.Eq(x => x.IndexerId, indexerId);

            await _collection.DeleteManyAsync(filter);
        }
    }
}
