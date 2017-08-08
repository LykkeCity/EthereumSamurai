using AutoMapper;
using EthereumSamurai.Core;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.MongoDb.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.MongoDb.Repositories
{
    public class Erc20ContractRepository : IErc20ContractRepository
    {
        private readonly IBaseSettings _baseSettings;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Erc20ContractEntity> _collection;
        private readonly IMapper _mapper;

        public Erc20ContractRepository(IBaseSettings baseSettings, IMongoDatabase database, IMapper mapper)
        {
            _baseSettings = baseSettings;
            _database = database;
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
            var filter = Builders<Erc20ContractEntity>.Filter.Eq(x => x.Address, contractAddress);
            IAsyncCursor<Erc20ContractEntity> cursor = await _collection.FindAsync(filter);
            Erc20ContractEntity result = await cursor.FirstOrDefaultAsync();
            Erc20ContractModel erc20ContractModel = _mapper.Map<Erc20ContractModel>(result);

            return erc20ContractModel;
        }

        public async Task SaveAsync(Erc20ContractModel erc20ContractModel)
        {
            Erc20ContractEntity contractEntity = _mapper.Map<Erc20ContractEntity>(erc20ContractModel);
            await _collection.DeleteOneAsync((x) => x.Address == contractEntity.Address);
            await _collection.InsertOneAsync(contractEntity);
        }

        public async Task DeleteAllForBlockNumberAsync(ulong blockNumber)
        {
            var filter = Builders<Erc20ContractEntity>.Filter.Eq(x => x.DeploymentBlockNumber, blockNumber);

            await _collection.DeleteManyAsync(filter);
        }

        public async Task SaveManyForBlockAsync(IEnumerable<Erc20ContractModel> contracts, ulong blockNumber)
        {
            if (contracts.Count() == 0)
            {
                await _collection.DeleteManyAsync((x) => x.DeploymentBlockNumber == blockNumber);

                return;
            }

            var entities = new List<Erc20ContractEntity>(contracts.Count());
            foreach (var contractModel in contracts)
            {
                Erc20ContractEntity transactionEntity = _mapper.Map<Erc20ContractEntity>(contractModel);
                entities.Add(transactionEntity);
            }

            await _collection.DeleteManyAsync((x) => x.DeploymentBlockNumber == blockNumber);
            await _collection.InsertManyAsync(entities);
        }

        public async Task DeleteByHash(string trHash)
        {
            var filter = Builders<Erc20ContractEntity>.Filter.Eq(x => x.DeploymentTranactionHash, trHash);

            await _collection.DeleteOneAsync(filter);
        }
    }
}
