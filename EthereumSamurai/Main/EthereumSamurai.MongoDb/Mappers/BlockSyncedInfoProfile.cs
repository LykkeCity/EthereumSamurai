using AutoMapper;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Indexing;
using EthereumSamurai.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace EthereumSamurai.MongoDb.Mappers
{
    public class BlockSyncedInfoProfile : Profile
    {
        public BlockSyncedInfoProfile()
        {
            CreateMap<BlockSyncedInfoModel, BlockSyncedInfoEntity>()
                .ForMember(dest => dest.IndexerId, opt => opt.MapFrom(src => src.IndexerId))
                .ForMember(dest => dest.BlockNumber, opt => opt.MapFrom(src => src.BlockNumber))
                .ReverseMap();
        }
    }
}
