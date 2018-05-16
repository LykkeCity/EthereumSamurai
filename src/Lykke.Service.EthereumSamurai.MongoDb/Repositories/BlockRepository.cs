﻿using AutoMapper;
using Lykke.Service.EthereumSamurai.Core;
using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Settings;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.MongoDb.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.MongoDb.Repositories
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
                new CreateIndexModel<BlockEntity>(Builders<BlockEntity>.IndexKeys.Ascending(x => x.BlockHash)),
                new CreateIndexModel<BlockEntity>(Builders<BlockEntity>.IndexKeys.Descending(x => x.Timestamp)),
                new CreateIndexModel<BlockEntity>(Builders<BlockEntity>.IndexKeys.Ascending(x => x.IsIndexed)),
            });

            _mapper = mapper;
        }

        public async Task SaveAsync(BlockModel blockModel)
        {
            BlockEntity blockEntity = _mapper.Map<BlockEntity>(blockModel);
            await _collection.DeleteOneAsync((x) => x.Number == blockEntity.Number);
            await _collection.InsertOneAsync(blockEntity);
        }

        public async Task<bool> DoesBlockExistAsync(string blockHash)
        {
            var filterBuilder = Builders<BlockEntity>.Filter;
            FilterDefinition<BlockEntity> filter = filterBuilder.Eq(x => x.BlockHash, blockHash);
            BlockEntity blockEntity = await _collection.Find(filter).FirstOrDefaultAsync();

            return blockEntity != null;
        }

        public async Task<BlockModel> GetForHashAsync(string blockHash)
        {
            var filterBuilder = Builders<BlockEntity>.Filter;
            FilterDefinition<BlockEntity> filter = filterBuilder.Eq(x => x.BlockHash, blockHash);

            BlockEntity blockEntity = await _collection.Find(filter).FirstOrDefaultAsync();
            BlockModel blockModel = _mapper.Map<BlockModel>(blockEntity);

            return blockModel;
        }

        public async Task<BlockModel> GetForNumberAsync(ulong number)
        {
            var filterBuilder = Builders<BlockEntity>.Filter;
            FilterDefinition<BlockEntity> filter = filterBuilder.Eq(x => x.Number, number);

            BlockEntity blockEntity = await _collection.Find(filter).FirstOrDefaultAsync();
            BlockModel blockModel = _mapper.Map<BlockModel>(blockEntity);

            return blockModel;
        }

        public async Task<BigInteger> GetSyncedBlocksCountAsync()
        {
            var filterBuilder = Builders<BlockEntity>.Filter;
            BigInteger result = await _collection.CountAsync(filterBuilder.Empty);

            return result;
        }

        public async Task<BigInteger> GetLastSyncedBlockAsync()
        {
            var sort = Builders<BlockEntity>.Sort.Descending(x => x.Number); //build sort object   
            BlockEntity result = _collection.Find<BlockEntity>(x => true).Sort(sort).FirstOrDefault();

            return new BigInteger(result?.Number ?? 1);
        }

        public async Task<BigInteger> GetNotSyncedBlocks(int take = 1000)
        {
            var sort = Builders<BlockEntity>.Sort.Ascending(x => x.IsIndexed); //build sort object   
            var query = _collection.Find<BlockEntity>(x => true).Sort(sort).Limit(take);
            var result = await query.ToListAsync();

            return result.Where(x => !x.IsIndexed);
        }
    }
}
