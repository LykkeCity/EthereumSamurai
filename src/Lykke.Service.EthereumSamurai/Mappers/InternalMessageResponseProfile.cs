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
    public class InternalMessageResponseProfile : Profile
    {
        public InternalMessageResponseProfile()
        {
            CreateMap<InternalMessageModel, InternalMessageResponse>()
            .ForMember(dest => dest.TransactionHash, opt => opt.MapFrom(src => src.TransactionHash))
            .ForMember(dest => dest.Depth, opt => opt.MapFrom(src => src.Depth))
            .ForMember(dest => dest.FromAddress, opt => opt.MapFrom(src => src.FromAddress))
            .ForMember(dest => dest.ToAddress, opt => opt.MapFrom(src => src.ToAddress))
            .AfterMap((transactionModel, transactionEntity) =>
            {
                transactionEntity.BlockNumber = (ulong)transactionModel.BlockNumber;
                transactionEntity.Value = transactionModel.Value.ToString();
                transactionEntity.Type = transactionModel.Type.ToString();
                transactionEntity.BlockTimeStamp = (uint)transactionModel.BlockTimestamp;
            })
            .ReverseMap();
        }
    }
}
