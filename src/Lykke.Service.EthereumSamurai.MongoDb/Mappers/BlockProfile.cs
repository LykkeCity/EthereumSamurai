using AutoMapper;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.MongoDb.Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Service.EthereumSamurai.MongoDb.Mappers
{
    public class BlockProfile : Profile
    {
        public BlockProfile()
        {
            CreateMap<BlockModel, BlockEntity>()
                .ForMember(dest => dest.BlockHash, opt => opt.MapFrom(src => src.BlockHash))
                .ForMember(dest => dest.ExtraData, opt => opt.MapFrom(src => src.ExtraData))
                .ForMember(dest => dest.LogsBloom, opt => opt.MapFrom(src => src.LogsBloom))
                .ForMember(dest => dest.Miner, opt => opt.MapFrom(src => src.Miner))
                .ForMember(dest => dest.Nonce, opt => opt.MapFrom(src => src.Nonce))
                .ForMember(dest => dest.ParentHash, opt => opt.MapFrom(src => src.ParentHash))
                .ForMember(dest => dest.ReceiptsRoot, opt => opt.MapFrom(src => src.ReceiptsRoot))
                .ForMember(dest => dest.Sha3Uncles, opt => opt.MapFrom(src => src.Sha3Uncles))
                .ForMember(dest => dest.StateRoot, opt => opt.MapFrom(src => src.StateRoot))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.TransactionsCount, opt => opt.MapFrom(src => src.TransactionsCount))
                .ForMember(dest => dest.TransactionsRoot, opt => opt.MapFrom(src => src.TransactionsRoot))
                .AfterMap((blockModel, blockEntity) =>
                {
                    blockEntity.GasUsed = blockModel.GasUsed.ToString();
                    blockEntity.GasLimit = blockModel.GasLimit.ToString();
                    blockEntity.Difficulty = blockModel.Difficulty.ToString();
                    blockEntity.TotalDifficulty = blockModel.TotalDifficulty.ToString();
                    blockEntity.Number = (ulong)blockModel.Number;
                    blockEntity.Size = blockModel.Size.ToString();
                    //blockEntity.IsIndexed = blockModel.IsIndexed;
                })
                .ReverseMap()
                .AfterMap((blockEntity, blockModel) =>
                {
                    blockModel.GasUsed = BigInteger.Parse(blockEntity.GasUsed);
                    blockModel.GasLimit = BigInteger.Parse(blockEntity.GasLimit);
                    blockModel.Difficulty = BigInteger.Parse(blockEntity.Difficulty);
                    blockModel.TotalDifficulty = BigInteger.Parse(blockEntity.TotalDifficulty);
                    blockModel.Number = new BigInteger(blockEntity.Number);
                    blockModel.Size = BigInteger.Parse(blockEntity.Size);
                    //blockModel.IsIndexed = blockEntity.IsIndexed;
                }).ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
