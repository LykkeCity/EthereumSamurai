using System.Numerics;
using AutoMapper;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.MongoDb.Entities;

namespace EthereumSamurai.MongoDb.Mappers
{
    public class Erc20BalanceProfile : Profile
    {
        public Erc20BalanceProfile()
        {
            CreateMap<Erc20BalanceEntity, Erc20BalanceModel>()
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => BigInteger.Parse(src.Balance)));
        }
    }
}