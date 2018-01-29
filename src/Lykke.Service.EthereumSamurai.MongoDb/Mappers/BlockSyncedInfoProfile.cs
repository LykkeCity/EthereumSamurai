using AutoMapper;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Indexing;
using Lykke.Service.EthereumSamurai.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Service.EthereumSamurai.MongoDb.Mappers
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
