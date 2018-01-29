using System;
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
                    Builders<Erc20TransferHistoryEntity>.IndexKeys.Combine
                    (
                        Builders<Erc20TransferHistoryEntity>.IndexKeys.Ascending(x => x.From),
                        Builders<Erc20TransferHistoryEntity>.IndexKeys.Ascending(x => x.To),
                        Builders<Erc20TransferHistoryEntity>.IndexKeys.Ascending(x => x.ContractAddress)
                    )
                )
            });

            _mapper = mapper;
        }

        public async Task DeleteAllForHash(string trHash)
        {
            var filter = Builders<Erc20TransferHistoryEntity>.Filter.Eq(x => x.TransactionHash, trHash);

            await _historyCollection.DeleteManyAsync(filter);
        }

        public async Task<IEnumerable<Erc20TransferHistoryModel>> GetAsync(Erc20TransferHistoryQuery query)
        {
            var filterBuilder = Builders<Erc20TransferHistoryEntity>.Filter;
            var filter        = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(query.TransactionHash))
            {
                filter &= filterBuilder.Eq(x => x.TransactionHash, query.TransactionHash);
            }

            if (query.BlockNumber.HasValue)
            {
                filter &= filterBuilder.Eq(x => x.BlockNumber, query.BlockNumber.Value);
            }

            if (!string.IsNullOrEmpty(query.AssetHolder))
            {
                filter &= filterBuilder.Or
                (
                    filterBuilder.Eq(x => x.From, query.AssetHolder),
                    filterBuilder.Eq(x => x.To, query.AssetHolder)
                );
            }

            if (query.Contracts != null && query.Contracts.Any())
            {
                filter &= filterBuilder.In(x => x.ContractAddress, query.Contracts.ToList());
            }
            
            var entities = await _historyCollection
                .Find(filter)
                .SortByDescending(x => x.BlockNumber)
                .ThenBy(x => x.TransactionIndex)
                .ThenBy(x => x.LogIndex)
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
            
            await _historyCollection.DeleteManyAsync(x => x.BlockNumber == blockNumber);

            if (blockTransferHistory.Any())
            {
                await _historyCollection.InsertManyAsync
                (
                    blockTransferHistory.Select(_mapper.Map<Erc20TransferHistoryEntity>)
                );
            }
        }
    }
}