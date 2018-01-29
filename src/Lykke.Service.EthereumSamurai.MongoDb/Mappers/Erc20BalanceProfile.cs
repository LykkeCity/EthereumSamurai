using System.Numerics;
using AutoMapper;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.MongoDb.Entities;

namespace Lykke.Service.EthereumSamurai.MongoDb.Mappers
{
    public class Erc20BalanceProfile : Profile
    {
        public Erc20BalanceProfile()
        {
            CreateMap<Erc20BalanceEntity, Erc20BalanceModel>()
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => BigInteger.Parse(src.Balance)));

            CreateMap<Erc20BalanceHistoryEntity, Erc20BalanceModel>()
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => BigInteger.Parse(src.Balance)));

            CreateMap<Erc20BalanceModel, Erc20BalanceEntity>()
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance.ToString()))
                .ForMember(dest => dest.BlockNumber, opt => opt.Ignore());

            CreateMap<Erc20BalanceModel, Erc20BalanceHistoryEntity>()
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance.ToString()))
                .ForMember(dest => dest.BlockNumber, opt => opt.Ignore());

            
        }
    }
}