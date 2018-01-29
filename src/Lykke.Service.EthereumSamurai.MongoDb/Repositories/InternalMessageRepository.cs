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
    public class InternalMessageRepository : IInternalMessageRepository
    {
        private readonly IBaseSettings _baseSettings;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<InternalMessageEntity> _collection;
        private readonly IMapper _mapper;

        public InternalMessageRepository(IBaseSettings baseSettings, IMongoDatabase database, IMapper mapper)
        {
            _baseSettings = baseSettings;
            _database = database;
            _collection = database.GetCollection<InternalMessageEntity>(Constants.InternalMessageCollectionName);

            _collection.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<InternalMessageEntity>(Builders<InternalMessageEntity>.IndexKeys.Ascending(x => x.TransactionHash)),
                new CreateIndexModel<InternalMessageEntity>(Builders<InternalMessageEntity>.IndexKeys.Ascending(x => x.BlockNumber)),
                new CreateIndexModel<InternalMessageEntity>(Builders<InternalMessageEntity>.IndexKeys.Ascending(x => x.ToAddress)),
                new CreateIndexModel<InternalMessageEntity>(Builders<InternalMessageEntity>.IndexKeys.Ascending(x => x.FromAddress)),
                new CreateIndexModel<InternalMessageEntity>(Builders<InternalMessageEntity>.IndexKeys.Combine(
                    Builders<InternalMessageEntity>.IndexKeys.Ascending(x => x.TransactionHash),
                    Builders<InternalMessageEntity>.IndexKeys.Ascending(x => x.MessageIndex)))
            });

            _mapper = mapper;
        }

        public async Task DeleteAllForHash(string transactionHash)
        {
            var filter = Builders<InternalMessageEntity>.Filter.Eq(x => x.TransactionHash, transactionHash);

            await _collection.DeleteManyAsync(filter);
        }

        public async Task DeleteAllForBlockNumberAsync(ulong blockNumber)
        {
            var filter = Builders<InternalMessageEntity>.Filter.Eq(x => x.BlockNumber, blockNumber);

            await _collection.DeleteManyAsync(filter);
        }

        public async Task SaveAsync(InternalMessageModel internalMessageModel)
        {
            InternalMessageEntity internalMessageEntity = _mapper.Map<InternalMessageEntity>(internalMessageModel);

            await _collection.DeleteOneAsync((x) => x.TransactionHash == internalMessageEntity.TransactionHash
            && x.MessageIndex == internalMessageModel.MessageIndex);
            await _collection.InsertOneAsync(internalMessageEntity);
        }

        public async Task SaveManyForBlockAsync(IEnumerable<InternalMessageModel> internalMessages, ulong blockNumber)
        {
            if (internalMessages.Count() == 0)
            {
                await _collection.DeleteManyAsync((x) => x.BlockNumber == blockNumber);
                return;
            }

            var entities = new List<InternalMessageEntity>(internalMessages.Count());

            foreach (var internalMessage in internalMessages)
            {
                InternalMessageEntity internalMessageEntity = _mapper.Map<InternalMessageEntity>(internalMessage);
                entities.Add(internalMessageEntity);
            }


            await _collection.DeleteManyAsync(x => x.BlockNumber == blockNumber);
            await _collection.InsertManyAsync(entities);
        }

        public async Task<IEnumerable<InternalMessageModel>> GetAsync(string transactionHash)
        {
            List<InternalMessageModel> result = new List<InternalMessageModel>();
            var filter = Builders<InternalMessageEntity>.Filter.Eq(x => x.TransactionHash, transactionHash);
            IAsyncCursor<InternalMessageEntity> cursor = await _collection.FindAsync(filter);

            await cursor.ForEachAsync((internalMessage) =>
            {
                InternalMessageModel internalMessageModel = _mapper.Map<InternalMessageModel>(internalMessage);
                result.Add(internalMessageModel);
            });

            return result;
        }

        public async Task<IEnumerable<InternalMessageModel>> GetAllByFilterAsync(InternalMessageQuery internalMessageQuery)
        {
            List<InternalMessageModel> result = new List<InternalMessageModel>();
            var filterBuilder = Builders<InternalMessageEntity>.Filter;
            var filter = Builders<InternalMessageEntity>.Filter.Empty;

            if (!string.IsNullOrEmpty(internalMessageQuery.FromAddress))
            {
                FilterDefinition<InternalMessageEntity> filterFrom = filterBuilder.Eq(x => x.FromAddress, internalMessageQuery.FromAddress);
                filter = filter & filterFrom;
            }

            if (!string.IsNullOrEmpty(internalMessageQuery.ToAddress))
            {
                FilterDefinition<InternalMessageEntity> filterTo = filterBuilder.Eq(x => x.ToAddress, internalMessageQuery.ToAddress);

                filter = internalMessageQuery.FromAddress == internalMessageQuery.ToAddress ? filter | filterTo : filter & filterTo;
            }

            if (internalMessageQuery.StartBlock.HasValue)
            {
                FilterDefinition<InternalMessageEntity> filterStartBlock =
                    filterBuilder.Gte(x => x.BlockNumber, internalMessageQuery.StartBlock.Value);
                filter = filter & filterStartBlock;
            }

            if (internalMessageQuery.StopBlock.HasValue)
            {
                FilterDefinition<InternalMessageEntity> filterEndBlock =
                    filterBuilder.Lte(x => x.BlockNumber, internalMessageQuery.StopBlock.Value);
                filter = filter & filterEndBlock;
            }

            var sort = Builders<InternalMessageEntity>.Sort
                .Descending(x => x.BlockNumber)
                .Ascending(x => x.TransactionHash)
                .Ascending(x => x.MessageIndex);
            MongoDB.Driver.IFindFluent<InternalMessageEntity, InternalMessageEntity> search = _collection.Find(filter);
            search = search.Sort(sort);

            internalMessageQuery.Start = internalMessageQuery.Start.HasValue ? internalMessageQuery.Start : 0;
            internalMessageQuery.Count = internalMessageQuery.Count.HasValue && internalMessageQuery.Count != 0 ? internalMessageQuery.Count : (int)await search.CountAsync();
            search = search.Skip(internalMessageQuery.Start).Limit(internalMessageQuery.Count);
            result = new List<InternalMessageModel>(internalMessageQuery.Count.Value);
            IAsyncCursor<InternalMessageEntity> cursor = await search.ToCursorAsync();

            await cursor.ForEachAsync((internalMessage) =>
            {
                InternalMessageModel internalMessageModel = _mapper.Map<InternalMessageModel>(internalMessage);
                result.Add(internalMessageModel);
            });

            return result;
        }
    }
}
