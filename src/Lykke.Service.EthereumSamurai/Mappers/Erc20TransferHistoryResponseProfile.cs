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
    public class Erc20TransferHistoryResponseProfile : Profile
    {
        public Erc20TransferHistoryResponseProfile()
        {
            CreateMap<Erc20TransferHistoryModel, Erc20TransferHistoryResponse>()
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
            .ForMember(dest => dest.ContractAddress, opt => opt.MapFrom(src => src.ContractAddress))
            .AfterMap((transactionModel, transactionEntity) =>
            {
                transactionEntity.LogIndex = transactionModel.LogIndex;
                transactionEntity.TransactionIndex = transactionModel.TransactionIndex;
                transactionEntity.BlockNumber = transactionModel.BlockNumber;
                transactionEntity.GasPrice = transactionModel.GasPrice;
                transactionEntity.BlockTimestamp = transactionModel.BlockTimestamp;
                transactionEntity.GasUsed = transactionModel.GasUsed;
                transactionEntity.TransferAmount = transactionModel.TransferAmount.ToString();
            })
            .ReverseMap();
        }
    }
}
