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
    public class Erc20BalanceRepository : IErc20BalanceRepository
    {
        private readonly IBaseSettings                               _baseSettings;
        private readonly IMongoCollection<Erc20BalanceEntity>        _balanceCollection;
        private readonly IMapper                                     _mapper;

        public Erc20BalanceRepository(IBaseSettings baseSettings, IMongoDatabase database, IMapper mapper)
        {
            _baseSettings      = baseSettings;
            _balanceCollection = database.GetCollection<Erc20BalanceEntity>(Constants.Erc20BalanceCollectionName);
            _mapper            = mapper;
        }

        public async Task<IEnumerable<Erc20BalanceModel>> GetAsync(Erc20BalanceQuery query)
        {
            var filterBuilder = Builders<Erc20BalanceEntity>.Filter;
            var filter = filterBuilder.Empty;

            if (query.BlockNumber.HasValue)
            {
                filter &= filterBuilder.Eq(x => x.BlockNumber, query.BlockNumber.Value);
            }

            if (!string.IsNullOrEmpty(query.AssetHolder))
            {
                filter &= filterBuilder.Eq(x => x.AssetHolderAddress, query.AssetHolder);
            }
            
            if (query.Contracts != null && query.Contracts.Any())
            {
                filter &= filterBuilder.In(x => x.ContractAddress, query.Contracts.ToList());
            }
            
            return (await _balanceCollection.Find(filter)
                .SortByDescending(x => x.BlockNumber)
                .ThenBy(x => x.ContractAddress)
                .ThenBy(x => x.AssetHolderAddress)
                .Skip(query.Start)
                .Limit(query.Count)
                .ToListAsync())
                .Select(_mapper.Map<Erc20BalanceModel>);
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
        }
    }
}