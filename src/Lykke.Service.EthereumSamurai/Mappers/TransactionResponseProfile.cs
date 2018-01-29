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
    public class TransactionResponseProfile : Profile
    {
        public TransactionResponseProfile()
        {
            CreateMap<TransactionModel, TransactionResponse>()
            .ForMember(dest => dest.TransactionHash, opt => opt.MapFrom(src => src.TransactionHash))
            .ForMember(dest => dest.BlockHash, opt => opt.MapFrom(src => src.BlockHash))
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
            .ForMember(dest => dest.Input, opt => opt.MapFrom(src => src.Input))
            .ForMember(dest => dest.ContractAddress, opt => opt.MapFrom(src => src.ContractAddress))
            .ForMember(dest => dest.HasError, opt => opt.MapFrom(src => src.HasError))
            .AfterMap((transactionModel, transactionEntity) =>
            {
                transactionEntity.TransactionIndex = (int)transactionModel.TransactionIndex;
                transactionEntity.BlockNumber = (ulong)transactionModel.BlockNumber;
                transactionEntity.Gas = transactionModel.Gas.ToString();
                transactionEntity.GasPrice = transactionModel.GasPrice.ToString();
                transactionEntity.Value = transactionModel.Value.ToString();
                transactionEntity.Nonce = transactionModel.Nonce.ToString();
                transactionEntity.BlockTimestamp = (int)transactionModel.BlockTimestamp;
                transactionEntity.GasUsed = transactionModel.GasUsed.ToString();
            })
            .ReverseMap();
        }
    }
}
