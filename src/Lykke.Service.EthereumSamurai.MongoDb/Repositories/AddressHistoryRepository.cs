using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.EthereumSamurai.Core;
using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Settings;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;
using Lykke.Service.EthereumSamurai.MongoDb.Entities;
using MongoDB.Driver;

namespace Lykke.Service.EthereumSamurai.MongoDb.Repositories
{
    public class AddressHistoryRepository : IAddressHistoryRepository
    {
        private readonly IBaseSettings                          _baseSettings;
        private readonly IMongoCollection<AddressHistoryEntity> _collection;
        private readonly IMongoDatabase                         _database;
        private readonly IMapper                                _mapper;

        public AddressHistoryRepository(IBaseSettings baseSettings, IMongoDatabase database, IMapper mapper)
        {
            _baseSettings = baseSettings;
            _database = database;
            _collection = database.GetCollection<AddressHistoryEntity>(Constants.AddressHistoryCollectionName);

            _collection.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<AddressHistoryEntity>
                (
                    Builders<AddressHistoryEntity>.IndexKeys.Ascending(x => x.TransactionHash)
                ),
                new CreateIndexModel<AddressHistoryEntity>
                (
                    Builders<AddressHistoryEntity>.IndexKeys.Ascending(x => x.BlockNumber)
                ),
                new CreateIndexModel<AddressHistoryEntity>
                (
                    Builders<AddressHistoryEntity>.IndexKeys.Ascending(x => x.From)
                ),
                new CreateIndexModel<AddressHistoryEntity>
                (
                    Builders<AddressHistoryEntity>.IndexKeys.Ascending(x => x.To)
                ),
                new CreateIndexModel<AddressHistoryEntity>(Builders<AddressHistoryEntity>.IndexKeys.Combine
                (
                    Builders<AddressHistoryEntity>.IndexKeys.Descending(x => x.BlockNumber),
                    Builders<AddressHistoryEntity>.IndexKeys.Ascending(x => x.TransactionIndex),
                    Builders<AddressHistoryEntity>.IndexKeys.Ascending(x => x.MessageIndex))
                )
            });

            _mapper = mapper;
        }

        public async Task DeleteByHash(string hash)
        {
            await _collection.DeleteManyAsync(x => x.TransactionHash == hash);
        }

        public async Task<IEnumerable<AddressHistoryModel>> GetAsync(AddressHistoryQuery addressHistoryQuery)
        {
            var filterBuilder = Builders<AddressHistoryEntity>.Filter;
            var filter        = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(addressHistoryQuery.FromAddress))
            {
                filter &= filterBuilder.Eq(x => x.From, addressHistoryQuery.FromAddress);
            }

            if (!string.IsNullOrEmpty(addressHistoryQuery.ToAddress))
            {
                var filterTo = filterBuilder.Eq(x => x.To, addressHistoryQuery.ToAddress);

                filter = addressHistoryQuery.FromAddress == addressHistoryQuery.ToAddress
                    ? filter | filterTo
                    : filter & filterTo;
            }
            
            if (addressHistoryQuery.StartBlock.HasValue)
            {
                var unixTime = addressHistoryQuery.StartBlock.Value;

                filter &= filterBuilder.Gte(x => x.BlockNumber, unixTime);
            }

            if (addressHistoryQuery.StopBlock.HasValue)
            {
                var unixTime = addressHistoryQuery.StopBlock.Value;
                
                filter &= filterBuilder.Lte(x => x.BlockNumber, unixTime);
            }

            var sort = Builders<AddressHistoryEntity>.Sort.Combine
            (
                Builders<AddressHistoryEntity>.Sort.Descending(x => x.BlockNumber),
                Builders<AddressHistoryEntity>.Sort.Ascending(x => x.TransactionIndex),
                Builders<AddressHistoryEntity>.Sort.Ascending(x => x.MessageIndex)
            );

            var search = _collection.Find(filter);

            search = search.Sort(sort);

            addressHistoryQuery.Start = addressHistoryQuery.Start ?? 0;
            addressHistoryQuery.Count = addressHistoryQuery.Count.HasValue && addressHistoryQuery.Count != 0
                ? addressHistoryQuery.Count
                : (int) await search.CountAsync();

            search = search.Skip(addressHistoryQuery.Start).Limit(addressHistoryQuery.Count);

            return (await search.ToListAsync())
                .Select(_mapper.Map<AddressHistoryModel>);
        }

        public async Task SaveManyForBlockAsync(IEnumerable<AddressHistoryModel> addressHistoryModels, ulong blockNumber)
        {
            if (!addressHistoryModels.Any())
            {
                return;
            }
            
            var entities = addressHistoryModels.Select(_mapper.Map<AddressHistoryEntity>);

            await _collection.DeleteManyAsync(x => x.BlockNumber == blockNumber);
            await _collection.InsertManyAsync(entities);
        }
    }
}