using AutoMapper;
using Lykke.Service.EthereumSamurai.Core;
using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.MongoDb.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models.Query;


namespace Lykke.Service.EthereumSamurai.MongoDb.Repositories
{
    public class Erc20ContractRepository : IErc20ContractRepository
    {
        private readonly IMongoCollection<Erc20ContractEntity> _collection;
        private readonly IMapper                               _mapper;

        public Erc20ContractRepository(
            IMongoDatabase database,
            IMapper mapper)
        {
            _collection = database.GetCollection<Erc20ContractEntity>(Constants.Erc20ContractCollectionName);

            _collection.Indexes.CreateMany(new[]
            {
                new CreateIndexModel<Erc20ContractEntity>(Builders<Erc20ContractEntity>.IndexKeys.Ascending(x => x.DeployerAddress)),
                new CreateIndexModel<Erc20ContractEntity>(Builders<Erc20ContractEntity>.IndexKeys.Ascending(x => x.DeploymentBlockNumber)),
                new CreateIndexModel<Erc20ContractEntity>(Builders<Erc20ContractEntity>.IndexKeys.Ascending(x => x.DeploymentTranactionHash)),
            });

            _mapper = mapper;
        }

        public async Task<Erc20ContractModel> GetAsync(string contractAddress)
        {
            var filter             = Builders<Erc20ContractEntity>.Filter.Eq(x => x.Address, contractAddress);
            var cursor             = await _collection.FindAsync(filter);
            var result             = await cursor.FirstOrDefaultAsync();
            var erc20ContractModel = _mapper.Map<Erc20ContractModel>(result);

            return erc20ContractModel;
        }

        public async Task<IEnumerable<Erc20ContractModel>> GetAsync(Erc20ContractQuery query)
        {
            var filterBuilder = Builders<Erc20ContractEntity>.Filter;
            var filter        = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(query.Address))
            {
                filter &= filterBuilder.Eq(x => x.Address, query.Address);
            }
            else if (!string.IsNullOrEmpty(query.NameOrSymbol))
            {
                filter &= filterBuilder.Or
                (
                    filterBuilder.Regex(x => x.TokenSymbol, query.NameOrSymbol),
                    filterBuilder.Regex(x => x.TokenName, query.NameOrSymbol)
                );
            }
            
            return (await _collection.Find(filter)
                .SortBy(x => x.TokenSymbol)
                .ThenBy(x => x.TokenName)
                .ThenBy(x => x.Address)
                .Skip(query.Start)
                .Limit(query.Count)
                .ToListAsync())
                .Select(_mapper.Map<Erc20ContractModel>);
        }

        public async Task SaveAsync(Erc20ContractModel erc20ContractModel)
        {
            var contractEntity = _mapper.Map<Erc20ContractEntity>(erc20ContractModel);
            
            await _collection.DeleteOneAsync(x => x.Address == contractEntity.Address);
            await _collection.InsertOneAsync(contractEntity);
        }

        public async Task DeleteAllForBlockNumberAsync(ulong blockNumber)
        {
            var filter = Builders<Erc20ContractEntity>.Filter.Eq(x => x.DeploymentBlockNumber, blockNumber);

            await _collection.DeleteManyAsync(filter);
        }

        public async Task SaveManyForBlockAsync(IEnumerable<Erc20ContractModel> contracts, ulong blockNumber)
        {
            if (!contracts.Any())
            {
                await _collection.DeleteManyAsync((x) => x.DeploymentBlockNumber == blockNumber);

                return;
            }

            var entities = new List<Erc20ContractEntity>(contracts.Count());

            foreach (var contractModel in contracts)
            {
                var transactionEntity = _mapper.Map<Erc20ContractEntity>(contractModel);

                entities.Add(transactionEntity);
            }

            try
            {
                await _collection.DeleteManyAsync((x) => x.DeploymentBlockNumber == blockNumber);
                await _collection.InsertManyAsync(entities);
            }
            catch(Exception)
            {
                foreach (var item in entities)
                {
                    await _collection.DeleteOneAsync((x) => x.Address == item.Address);
                }

                await _collection.InsertManyAsync(entities);
            }
        }

        public async Task DeleteByHash(string trHash)
        {
            var filter = Builders<Erc20ContractEntity>.Filter.Eq(x => x.DeploymentTranactionHash, trHash);

            await _collection.DeleteOneAsync(filter);
        }
    }
}
