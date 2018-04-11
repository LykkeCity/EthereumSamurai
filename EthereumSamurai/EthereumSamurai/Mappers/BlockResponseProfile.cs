//TODO: Cleanup code

using AutoMapper;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EthereumSamurai.Mappers
{
    public class BlockResponseProfile : Profile
    {
        public BlockResponseProfile()
        {
            CreateMap<BlockModel, BlockResponse>()
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.BlockHash, opt => opt.MapFrom(src => src.BlockHash))
            .ForMember(dest => dest.ParentHash, opt => opt.MapFrom(src => src.ParentHash))
            .ForMember(dest => dest.GasLimit, opt => opt.MapFrom(src => src.GasLimit))
            .ForMember(dest => dest.GasUsed, opt => opt.MapFrom(src => src.GasUsed))
            .ReverseMap();
        }
    }
}
