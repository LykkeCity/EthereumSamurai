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
        private readonly IBaseSettings                        _baseSettings;
        private readonly IMongoCollection<Erc20BalanceEntity> _collection;
        private readonly IMapper                              _mapper;

        public Erc20BalanceRepository(IBaseSettings baseSettings, IMongoDatabase database, IMapper mapper)
        {
            //_baseSettings = baseSettings;
            //_collection   = database.GetCollection<Erc20BalanceEntity>(Constants.Erc20BalanceCollectionName);
            //_mapper       = mapper;
        }
    }
}