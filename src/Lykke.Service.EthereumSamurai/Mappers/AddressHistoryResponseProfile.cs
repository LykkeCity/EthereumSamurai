//TODO: Cleanup code

using AutoMapper;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.Mappers
{
    public class AddressHistoryResponseProfile : Profile
    {
        public AddressHistoryResponseProfile()
        {
            CreateMap<AddressHistoryModel, AddressHistoryResponse>()
            .ForMember(dest => dest.TransactionHash, opt => opt.MapFrom(src => src.TransactionHash))
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
            .ForMember(dest => dest.HasError, opt => opt.MapFrom(src => src.HasError))
            .ForMember(dest => dest.MessageIndex, opt => opt.MapFrom(src => src.MessageIndex))
            .AfterMap((addressHistoryModel, addressHistoryEntity) =>
            {
                addressHistoryEntity.TransactionIndex = addressHistoryModel.TransactionIndex;
                addressHistoryEntity.BlockNumber = addressHistoryModel.BlockNumber;
                addressHistoryEntity.Value = addressHistoryModel.Value.ToString();
                addressHistoryEntity.GasPrice = addressHistoryModel.GasPrice.ToString();
                addressHistoryEntity.GasUsed = addressHistoryModel.GasUsed.ToString();
                addressHistoryEntity.BlockTimestamp = addressHistoryModel.BlockTimestamp;
            }).ReverseMap();
        }
    }
}
