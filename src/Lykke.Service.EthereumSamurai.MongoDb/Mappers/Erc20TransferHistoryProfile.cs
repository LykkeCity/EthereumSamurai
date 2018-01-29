using System.Numerics;
using AutoMapper;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.MongoDb.Entities;

namespace Lykke.Service.EthereumSamurai.MongoDb.Mappers
{
    public class Erc20TransferHistoryProfile : Profile
    {
        public Erc20TransferHistoryProfile()
        {
            CreateMap<Erc20TransferHistoryModel, Erc20TransferHistoryEntity>()
                .ForMember(x => x.TransferAmount, opt => opt.MapFrom(src => src.TransferAmount.ToString()))
                .ForMember(x => x.GasPrice, opt => opt.MapFrom(src => src.GasPrice.ToString()))
                .ForMember(x => x.GasUsed, opt => opt.MapFrom(src => src.GasUsed.ToString()));

            CreateMap<Erc20TransferHistoryEntity, Erc20TransferHistoryModel>()
                .ForMember(x => x.TransferAmount, opt => opt.MapFrom(src => BigInteger.Parse(src.TransferAmount)))
                .ForMember(x => x.GasPrice, opt => opt.MapFrom(src => BigInteger.Parse(src.GasPrice ?? "0")))
                .ForMember(x => x.GasUsed, opt => opt.MapFrom(src => BigInteger.Parse(src.GasUsed ?? "0")));
        }
    }
}