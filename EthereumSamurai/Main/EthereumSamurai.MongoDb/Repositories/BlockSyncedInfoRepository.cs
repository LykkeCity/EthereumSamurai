using System;
using System.Numerics;
using System.Threading.Tasks;
using AutoMapper;
using EthereumSamurai.Core;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.Models.Indexing;
using EthereumSamurai.MongoDb.Entities;
using MongoDB.Driver;
using System.Linq;

namespace EthereumSamurai.MongoDb.Repositories
{
    [Obsolete]
    public class BlockSyncedInfoRepository : IBlockSyncedInfoRepository
    {
        private readonly IBaseSettings                           _baseSettings;
        private readonly IMongoCollection<BlockSyncedInfoEntity> _collection;
        private readonly IMongoDatabase                          _database;
        private readonly IMapper                                 _mapper;

        public BlockSyncedInfoRepository(
            IBaseSettings  baseSettings,
            IMongoDatabase database,
            IMapper        mapper)
        {
            _baseSettings = baseSettings;
            _collection   = database.GetCollection<BlockSyncedInfoEntity>(Constants.BlockSyncedInfoCollectionName);
            _database     = database;

            _collection.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<BlockSyncedInfoEntity>
                (
                    Builders<BlockSyncedInfoEntity>.IndexKeys.Ascending(x => x.IndexerId)
                )
            });

            _mapper = mapper;
        }

        public async Task ClearAll()
        {
            var filterBuilder = Builders<BlockSyncedInfoEntity>.Filter;
            var filter        = filterBuilder.Empty;

            await _collection.DeleteManyAsync(filter);
        }

        public async Task ClearForIndexer(string indexerId)
        {
            var filterBuilder = Builders<BlockSyncedInfoEntity>.Filter;
            var filter        = filterBuilder.Eq(x => x.IndexerId, indexerId);

            await _collection.DeleteManyAsync(filter);
        }

        public async Task<BigInteger?> GetLastSyncedBlockForIndexerAsync(string indexerId)
        {
            var filterBuilder = Builders<BlockSyncedInfoEntity>.Filter;
            var filter        = filterBuilder.Eq(x => x.IndexerId, indexerId);
            var sort          = Builders<BlockSyncedInfoEntity>.Sort.Descending(x => x.BlockNumber);
            var result        = await _collection.Find(filter).Sort(sort).FirstOrDefaultAsync();

            return result != null 
                 ? new BigInteger?(result.BlockNumber)
                 : null;
        }

        public async Task SaveAsync(BlockSyncedInfoModel syncedBlockInfo)
        {
            var blockSyncedInfoEntity = _mapper.Map<BlockSyncedInfoEntity>(syncedBlockInfo);

            await _collection.DeleteOneAsync(x => x.BlockNumber == blockSyncedInfoEntity.BlockNumber);
            await _collection.InsertOneAsync(blockSyncedInfoEntity);
        }
    }
}