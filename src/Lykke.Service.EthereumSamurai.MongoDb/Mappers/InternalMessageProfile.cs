using AutoMapper;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Service.EthereumSamurai.MongoDb.Mappers
{
    public class InternalMessageProfile : Profile
    {
        public InternalMessageProfile()
        {
            CreateMap<InternalMessageModel, InternalMessageEntity>()
            .ForMember(dest => dest.MessageIndex, opt => opt.MapFrom(src => src.MessageIndex))
            .ForMember(dest => dest.TransactionHash, opt => opt.MapFrom(src => src.TransactionHash))
            .ForMember(dest => dest.BlockNumber, opt => opt.MapFrom(src => src.BlockNumber))
            .ForMember(dest => dest.Depth, opt => opt.MapFrom(src => src.Depth))
            .ForMember(dest => dest.FromAddress, opt => opt.MapFrom(src => src.FromAddress))
            .ForMember(dest => dest.ToAddress, opt => opt.MapFrom(src => src.ToAddress))
            .AfterMap((internalMessageModel, internalMessageEntity) =>
            {
                internalMessageEntity.Value = internalMessageModel.Value.ToString();
                internalMessageEntity.Type = (int)internalMessageModel.Type;
                internalMessageEntity.BlockTimestamp = (uint)internalMessageModel.BlockTimestamp;
            })
            .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<InternalMessageEntity, InternalMessageModel>()
            .ForMember(dest => dest.MessageIndex, opt => opt.MapFrom(src => src.MessageIndex))
            .ForMember(dest => dest.TransactionHash, opt => opt.MapFrom(src => src.TransactionHash))
            .ForMember(dest => dest.BlockNumber, opt => opt.MapFrom(src => src.BlockNumber))
            .ForMember(dest => dest.Depth, opt => opt.MapFrom(src => src.Depth))
            .ForMember(dest => dest.FromAddress, opt => opt.MapFrom(src => src.FromAddress))
            .ForMember(dest => dest.ToAddress, opt => opt.MapFrom(src => src.ToAddress))
             .AfterMap((internalMessageEntity, internalMessageModel) =>
             {
                 internalMessageModel.Type = (InternalMessageModelType)internalMessageEntity.Type;
                 internalMessageModel.Value = BigInteger.Parse(internalMessageEntity.Value);
                 internalMessageModel.BlockTimestamp = internalMessageEntity.BlockTimestamp;
             })
            .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
