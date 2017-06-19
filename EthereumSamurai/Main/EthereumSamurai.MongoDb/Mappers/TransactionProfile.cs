using AutoMapper;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace EthereumSamurai.MongoDb.Mappers
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
            .AfterMap((transactionModel, transactionEntity) =>
            {
                transactionEntity.TransactionIndex = transactionModel.TransactionIndex.ToString();
                transactionEntity.BlockNumber      = (ulong)transactionModel.BlockNumber;
                transactionEntity.Gas              = transactionModel.Gas.ToString();
                transactionEntity.GasPrice         = transactionModel.GasPrice.ToString();
                transactionEntity.Value            = transactionModel.Value.ToString();
                transactionEntity.Nonce            = transactionModel.Nonce.ToString();
                transactionEntity.BlockTimestamp   = (uint)transactionModel.BlockTimestamp;
            })
            .ReverseMap()
            .AfterMap((transactionEntity, transactionModel) =>
            {
                transactionModel.TransactionIndex = BigInteger.Parse(transactionEntity.TransactionIndex);
                transactionModel.BlockNumber = new BigInteger(transactionEntity.BlockNumber);
                transactionModel.Gas = BigInteger.Parse(transactionEntity.Gas);
                transactionModel.GasPrice = BigInteger.Parse(transactionEntity.GasPrice);
                transactionModel.Value = BigInteger.Parse(transactionEntity.Value);
                transactionModel.Nonce = BigInteger.Parse(transactionEntity.Nonce);
                transactionModel.BlockTimestamp = new BigInteger(transactionEntity.BlockTimestamp);
            });
        }
    }
}
