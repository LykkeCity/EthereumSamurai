﻿using System.Numerics;
using AutoMapper;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.MongoDb.Entities;

namespace EthereumSamurai.MongoDb.Mappers
{
    public class Erc20TransferHistoryProfile : Profile
    {
        public Erc20TransferHistoryProfile()
        {
            CreateMap<Erc20TransferHistoryModel, Erc20TransferHistoryEntity>()
                .ForMember(x => x.TransferAmount, opt => opt.MapFrom(src => src.TransferAmount.ToString()));

            CreateMap<Erc20TransferHistoryEntity, Erc20TransferHistoryModel>()
                .ForMember(x => x.TransferAmount, opt => opt.MapFrom(src => BigInteger.Parse(src.TransferAmount)));
        }
    }
}