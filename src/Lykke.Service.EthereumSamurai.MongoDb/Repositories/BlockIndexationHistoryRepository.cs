using System;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core;
using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Settings;
using Lykke.Service.EthereumSamurai.MongoDb.Entities;
using MongoDB.Driver;

namespace Lykke.Service.EthereumSamurai.MongoDb.Repositories
{
    public class BlockIndexationHistoryRepository : IBlockIndexationHistoryRepository
    {
        private readonly IBaseSettings                                  _baseSettings;
        private readonly IMongoCollection<BlockIndexationHistoryEntity> _collection;

        public BlockIndexationHistoryRepository(
            IBaseSettings  baseSettings,
            IMongoDatabase database)
        {
            _baseSettings = baseSettings;
            _collection   = database.GetCollection<BlockIndexationHistoryEntity>(Constants.BlockIndexationHistoryCollectionName);

            _collection.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<BlockIndexationHistoryEntity>
                (
                    Builders<BlockIndexationHistoryEntity>.IndexKeys.Combine
                    (
                        Builders<BlockIndexationHistoryEntity>.IndexKeys.Ascending(x => x.BalancesIndexerVersion),
                        Builders<BlockIndexationHistoryEntity>.IndexKeys.Ascending(x => x.BlockNumber)
                    )
                )
            });
        }

        public async Task<ulong?> GetLowestBlockWithNotIndexedBalances(ulong startFrom)
        {
            var filterBuilder = Builders<BlockIndexationHistoryEntity>.Filter;
            var filter        = filterBuilder.Eq(x => x.BalancesIndexerVersion, null);
            var sort          = Builders<BlockIndexationHistoryEntity>.Sort.Ascending(x => x.BlockNumber);

            var candidate = (await _collection.Find(filter).Sort(sort).FirstOrDefaultAsync())?.BlockNumber;

            if (candidate.HasValue)
            {
                if (candidate == startFrom + 1)
                {
                    return candidate;
                }


                if (await _collection.Find(x => x.BlockNumber == candidate - 1).AnyAsync())
                {
                    return candidate;
                }
            }

            return null;
        }


        public async Task MarkBalancesAsIndexed(ulong blockNumber, int jobVersion)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            await _collection.UpdateManyAsync
            (
                x => x.BalancesIndexerVersion != null && x.BlockNumber > blockNumber,
                Builders<BlockIndexationHistoryEntity>
                    .Update
                    .Set(x => x.BalancesIndexerVersion, null)
                    .Set(x => x.Timestamp, timestamp)
            );

            await _collection.FindOneAndUpdateAsync
            (
                x => x.BlockNumber == blockNumber,
                Builders<BlockIndexationHistoryEntity>
                    .Update
                    .Set(x => x.BalancesIndexerVersion, jobVersion)
                    .Set(x => x.Timestamp, timestamp)
            );
        }

        public async Task MarkBlockAsIndexed(ulong blockNumber, int jobVersion)
        {
            await _collection.DeleteOneAsync(x => x.BlockNumber == blockNumber);
            await _collection.InsertOneAsync(new BlockIndexationHistoryEntity
            {
                BlockIndexerVersion = jobVersion,
                BlockNumber         = blockNumber,
                Timestamp           = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            });
        }
    }
}