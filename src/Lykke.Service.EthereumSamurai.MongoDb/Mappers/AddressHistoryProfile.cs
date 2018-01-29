using AutoMapper;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Service.EthereumSamurai.MongoDb.Mappers
{
    public class AddressHistoryProfile : Profile
    {
        public AddressHistoryProfile()
        {
            CreateMap<AddressHistoryModel, AddressHistoryEntity>()
            .ForMember(dest => dest.TransactionHash, opt => opt.MapFrom(src => src.TransactionHash))
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
            .ForMember(dest => dest.HasError, opt => opt.MapFrom(src => src.HasError))
            .ForMember(dest => dest.MessageIndex, opt => opt.MapFrom(src => src.MessageIndex))
            .AfterMap((addressHistoryModel, addressHistoryEntity) =>
            {
                addressHistoryEntity.TransactionIndex = addressHistoryModel.TransactionIndex;
                addressHistoryEntity.BlockNumber = (ulong)addressHistoryModel.BlockNumber;
                addressHistoryEntity.Value = addressHistoryModel.Value.ToString();
                addressHistoryEntity.GasUsed = addressHistoryModel.GasUsed.ToString();
                addressHistoryEntity.GasPrice = addressHistoryModel.GasPrice.ToString();
                addressHistoryEntity.Value = addressHistoryModel.Value.ToString();
                addressHistoryEntity.BlockTimestamp = (uint)addressHistoryModel.BlockTimestamp;
            }).ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<AddressHistoryEntity, AddressHistoryModel>()
            .ForMember(dest => dest.TransactionHash, opt => opt.MapFrom(src => src.TransactionHash))
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
            .ForMember(dest => dest.HasError, opt => opt.MapFrom(src => src.HasError))
            .ForMember(dest => dest.MessageIndex, opt => opt.MapFrom(src => src.MessageIndex))
            .AfterMap((addressHistoryEntity, addressHistoryModel) =>
            {
                addressHistoryModel.TransactionIndex = addressHistoryEntity.TransactionIndex;
                addressHistoryModel.BlockNumber = addressHistoryEntity.BlockNumber;
                addressHistoryModel.Value = BigInteger.Parse(addressHistoryEntity.Value);
                addressHistoryModel.GasUsed = BigInteger.Parse(addressHistoryEntity.GasUsed ?? "0");
                addressHistoryModel.GasPrice = BigInteger.Parse(addressHistoryEntity.GasPrice ?? "0");
                addressHistoryModel.BlockTimestamp = addressHistoryEntity.BlockTimestamp;
            }).ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
