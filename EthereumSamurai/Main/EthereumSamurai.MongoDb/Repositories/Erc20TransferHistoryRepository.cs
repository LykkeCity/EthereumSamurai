using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EthereumSamurai.Core;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;
using EthereumSamurai.MongoDb.Entities;
using MongoDB.Driver;

namespace EthereumSamurai.MongoDb.Repositories
{
    public class Erc20TransferHistoryRepository : IErc20TransferHistoryRepository
    {
        private readonly IBaseSettings                                _baseSettings;
        private readonly IMapper                                      _mapper;
        private readonly IMongoCollection<Erc20TransferHistoryEntity> _historyCollection;



        public Erc20TransferHistoryRepository(IBaseSettings baseSettings, IMongoDatabase database, IMapper mapper)
        {
            _baseSettings      = baseSettings;
            _historyCollection = database.GetCollection<Erc20TransferHistoryEntity>(Constants.Erc20TransferHistoryCollectionName);
            
            _historyCollection.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<Erc20TransferHistoryEntity>
                (
                    Builders<Erc20TransferHistoryEntity>.IndexKeys.Combine
                    (
                        Builders<Erc20TransferHistoryEntity>.IndexKeys.Descending(x => x.BlockNumber),
                        Builders<Erc20TransferHistoryEntity>.IndexKeys.Ascending(x => x.TransactionIndex),
                        Builders<Erc20TransferHistoryEntity>.IndexKeys.Ascending(x => x.LogIndex)
                    )
                ),
                new CreateIndexModel<Erc20TransferHistoryEntity>
                (
                    Builders<Erc20TransferHistoryEntity>.IndexKeys.Ascending(x => x.ContractAddress)
                ),
                new CreateIndexModel<Erc20TransferHistoryEntity>
                (
                    Builders<Erc20TransferHistoryEntity>.IndexKeys.Ascending(x => x.TransactionHash)
                ),
                new CreateIndexModel<Erc20TransferHistoryEntity>
                (
                    Builders<Erc20TransferHistoryEntity>.IndexKeys.Combine
                    (
                        Builders<Erc20TransferHistoryEntity>.IndexKeys.Ascending(x => x.To),
                        Builders<Erc20TransferHistoryEntity>.IndexKeys.Ascending(x => x.ContractAddress)
                    )
                ),
                new CreateIndexModel<Erc20TransferHistoryEntity>
                (
                    Builders<Erc20TransferHistoryEntity>.IndexKeys.Combine
                    (
                        Builders<Erc20TransferHistoryEntity>.IndexKeys.Ascending(x => x.From),
                        Builders<Erc20TransferHistoryEntity>.IndexKeys.Ascending(x => x.ContractAddress)
                    )
                ),
            });

            _mapper = mapper;
        }


        public async Task<IEnumerable<Erc20TransferHistoryModel>> GetAsync(Erc20TransferHistoryQuery query)
        {
            var filterBuilder = Builders<Erc20TransferHistoryEntity>.Filter;
            var filter        = filterBuilder.Empty;

            if (string.IsNullOrEmpty(query.ContractAddress))
            {
                filter &= filterBuilder.Eq(x => x.ContractAddress, query.ContractAddress);
            }

            if (query.FromBlockNumber.HasValue)
            {
                filter &= filterBuilder.Gte(x => x.BlockNumber, query.FromBlockNumber.Value);
            }

            if (query.ToBlockNumber.HasValue)
            {
                filter &= filterBuilder.Lte(x => x.BlockNumber, query.ToBlockNumber.Value);
            }

            if (string.IsNullOrEmpty(query.TransactionHash))
            {
                filter &= filterBuilder.Eq(x => x.TransactionHash, query.TransactionHash);
            }

            if (string.IsNullOrEmpty(query.TransfereeAddress))
            {
                filter &= filterBuilder.Eq(x => x.To, query.TransfereeAddress);
            }

            if (string.IsNullOrEmpty(query.TransferorAddress))
            {
                filter &= filterBuilder.Eq(x => x.From, query.TransferorAddress);
            }

            var sort = Builders<Erc20TransferHistoryEntity>.Sort
                .Descending(x => x.BlockNumber)
                .Ascending(x => x.TransactionIndex)
                .Ascending(x => x.LogIndex);

            var entities = await _historyCollection
                .Find(filter)
                .Sort(sort)
                .Skip(query.Start)
                .Limit(query.Count)
                .ToListAsync();

            return entities.Select(_mapper.Map<Erc20TransferHistoryModel>);
        }

        public async Task SaveForBlockAsync(IEnumerable<Erc20TransferHistoryModel> blockTransferHistory, ulong blockNumber)
        {
            if (blockTransferHistory.Any(x => x.BlockNumber != blockNumber))
            {
                throw new InvalidOperationException("All transfers should be part of the same block.");
            }

            // TODO: Prevent pending balance updates for block
            // TODO: Undo processed balance updates for block

            await _historyCollection.DeleteManyAsync(x => x.BlockNumber == blockNumber);

            if (blockTransferHistory.Any())
            {
                // TODO: Schedule new balance updates for block
                
                await _historyCollection.InsertManyAsync
                (
                    blockTransferHistory.Select(_mapper.Map<Erc20TransferHistoryEntity>)
                );
            }
        }
    }
}