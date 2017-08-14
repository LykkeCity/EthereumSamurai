using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EthereumSamurai.Core;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.MongoDb.Entities;
using MongoDB.Driver;

namespace EthereumSamurai.MongoDb.Repositories
{
    public class Erc20BalanceRepository : IErc20BalanceRepository
    {
        private readonly IBaseSettings                               _baseSettings;
        private readonly IMongoCollection<Erc20BalanceEntity>        _balanceCollection;
        private readonly IMongoCollection<Erc20BalanceHistoryEntity> _historyCollection;
        private readonly IMapper                                     _mapper;

        public Erc20BalanceRepository(IBaseSettings baseSettings, IMongoDatabase database, IMapper mapper)
        {
            _baseSettings      = baseSettings;
            _balanceCollection = database.GetCollection<Erc20BalanceEntity>(Constants.Erc20BalanceCollectionName);
            _historyCollection = database.GetCollection<Erc20BalanceHistoryEntity>(Constants.Erc20BalanceHistoryCollectionName);
            _mapper            = mapper;

            _historyCollection.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<Erc20BalanceHistoryEntity>
                (
                    Builders<Erc20BalanceHistoryEntity>.IndexKeys.Descending(x => x.BlockNumber)
                ), 
                new CreateIndexModel<Erc20BalanceHistoryEntity>
                (
                    Builders<Erc20BalanceHistoryEntity>.IndexKeys.Combine
                    (
                        Builders<Erc20BalanceHistoryEntity>.IndexKeys.Ascending(x => x.AssetHolderAddress),
                        Builders<Erc20BalanceHistoryEntity>.IndexKeys.Ascending(x => x.ContractAddress)
                    )
                )
            });

            _historyCollection.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<Erc20BalanceHistoryEntity>
                (
                    Builders<Erc20BalanceHistoryEntity>.IndexKeys.Combine
                    (
                        Builders<Erc20BalanceHistoryEntity>.IndexKeys.Ascending(x => x.AssetHolderAddress),
                        Builders<Erc20BalanceHistoryEntity>.IndexKeys.Ascending(x => x.ContractAddress),
                        Builders<Erc20BalanceHistoryEntity>.IndexKeys.Descending(x => x.BlockNumber)
                    )
                )
            });
        }

        public async Task<IEnumerable<Erc20BalanceModel>> GetAsync(string assetHolderAddress, IEnumerable<string> contractAddresses)
        {
            var filterBuilder = Builders<Erc20BalanceEntity>.Filter;
            var filter = filterBuilder.Empty;

            filter &= filterBuilder.Eq(x => x.AssetHolderAddress, assetHolderAddress);
            filter &= filterBuilder.In(x => x.ContractAddress,    contractAddresses.ToList());

            return (await _balanceCollection.Find(filter).ToListAsync())
                .Select(_mapper.Map<Erc20BalanceModel>);
        }

        public async Task<Erc20BalanceModel> GetPreviousAsync(string assetHolderAddress, string contractAddress, ulong currentBlockNumber)
        {
            var filterBuilder = Builders<Erc20BalanceHistoryEntity>.Filter;
            var filter        = filterBuilder.Empty;

            filter &= filterBuilder.Eq(x => x.AssetHolderAddress, assetHolderAddress);
            filter &= filterBuilder.Eq(x => x.ContractAddress,    contractAddress);
            filter &= filterBuilder.Lt(x => x.BlockNumber,        currentBlockNumber);

            var sort = Builders<Erc20BalanceHistoryEntity>.Sort
                .Descending(x => x.BlockNumber);

            var balanceHistory = await _historyCollection
                .Find(filter)
                .Sort(sort)
                .FirstOrDefaultAsync();

            return balanceHistory != null
                 ? _mapper.Map<Erc20BalanceModel>(balanceHistory)
                 : null;
        }

        public async Task SaveForBlockAsync(IEnumerable<Erc20BalanceModel> balances, ulong blockNumber)
        {
            // Refresh current balances
            await _balanceCollection.DeleteManyAsync(x => x.BlockNumber >= blockNumber);

            Erc20BalanceEntity SetBlockNumberToBalanceEntity(Erc20BalanceEntity entity)
            {
                entity.BlockNumber = blockNumber;

                return entity;
            }

            var balanceEntities = balances
                .Select(_mapper.Map<Erc20BalanceEntity>)
                .Select(SetBlockNumberToBalanceEntity)
                .ToList();

            foreach (var balanceEntity in balanceEntities)
            {
                await _balanceCollection.ReplaceOneAsync
                (
                    x => x.AssetHolderAddress == balanceEntity.AssetHolderAddress 
                      && x.ContractAddress    == balanceEntity.ContractAddress,
                    balanceEntity,
                    new UpdateOptions
                    {
                        IsUpsert = true
                    }
                );
            }

            // Update balance history
            await _historyCollection.DeleteManyAsync(x => x.BlockNumber >= blockNumber);

            Erc20BalanceHistoryEntity SetBlockNumberToHistoryEntity(Erc20BalanceHistoryEntity entity)
            {
                entity.BlockNumber = blockNumber;

                return entity;
            }

            var historyEntities = balances
                .Select(_mapper.Map<Erc20BalanceHistoryEntity>)
                .Select(SetBlockNumberToHistoryEntity)
                .ToList();
            
            if (historyEntities.Any())
            {
                await _historyCollection.InsertManyAsync(historyEntities);
            }
        }
    }
}