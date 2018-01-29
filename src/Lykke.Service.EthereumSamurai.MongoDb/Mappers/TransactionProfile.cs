using AutoMapper;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Service.EthereumSamurai.MongoDb.Mappers
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<TransactionModel, TransactionEntity>()
            .ForMember(dest => dest.TransactionHash, opt => opt.MapFrom(src => src.TransactionHash))
            .ForMember(dest => dest.BlockHash, opt => opt.MapFrom(src => src.BlockHash))
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
            .ForMember(dest => dest.Input, opt => opt.MapFrom(src => src.Input))
            .ForMember(dest => dest.ContractAddress, opt => opt.MapFrom(src => src.ContractAddress))
            .ForMember(dest => dest.HasError, opt => opt.MapFrom(src => src.HasError))
            .AfterMap((transactionModel, transactionEntity) =>
            {
                transactionEntity.TransactionIndex = transactionModel.TransactionIndex.ToString();
                transactionEntity.BlockNumber = (ulong)transactionModel.BlockNumber;
                transactionEntity.Gas = transactionModel.Gas.ToString();
                transactionEntity.GasPrice = transactionModel.GasPrice.ToString();
                transactionEntity.Value = transactionModel.Value.ToString();
                transactionEntity.Nonce = transactionModel.Nonce.ToString();
                transactionEntity.BlockTimestamp = (uint)transactionModel.BlockTimestamp;
                transactionEntity.GasUsed = transactionModel.GasUsed.ToString();
            }).ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TransactionEntity, TransactionModel>()
            .ForMember(dest => dest.TransactionHash, opt => opt.MapFrom(src => src.TransactionHash))
            .ForMember(dest => dest.BlockHash, opt => opt.MapFrom(src => src.BlockHash))
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
            .ForMember(dest => dest.Input, opt => opt.MapFrom(src => src.Input))
            .ForMember(dest => dest.ContractAddress, opt => opt.MapFrom(src => src.ContractAddress))
            .ForMember(dest => dest.HasError, opt => opt.MapFrom(src => src.HasError))
            .AfterMap((transactionEntity, transactionModel) =>
            {
                transactionModel.TransactionIndex = BigInteger.Parse(transactionEntity.TransactionIndex);
                transactionModel.BlockNumber = new BigInteger(transactionEntity.BlockNumber);
                transactionModel.Gas = BigInteger.Parse(transactionEntity.Gas);
                transactionModel.GasPrice = BigInteger.Parse(transactionEntity.GasPrice);
                transactionModel.Value = BigInteger.Parse(transactionEntity.Value);
                transactionModel.Nonce = BigInteger.Parse(transactionEntity.Nonce);
                transactionModel.BlockTimestamp = new BigInteger(transactionEntity.BlockTimestamp);
                transactionModel.GasUsed = BigInteger.Parse(transactionEntity.GasUsed);
            }).ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
