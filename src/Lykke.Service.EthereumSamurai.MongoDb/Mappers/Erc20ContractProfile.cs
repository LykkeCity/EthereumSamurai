using System.Numerics;
using AutoMapper;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.MongoDb.Entities;

namespace Lykke.Service.EthereumSamurai.MongoDb.Mappers
{
    public class Erc20ContractProfile : Profile
    {
        public Erc20ContractProfile()
        {
            CreateMap<Erc20ContractModel, Erc20ContractEntity>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.DeployerAddress, opt => opt.MapFrom(src => src.DeployerAddress))
                .ForMember(dest => dest.TokenName, opt => opt.MapFrom(src => src.TokenName))
                .ForMember(dest => dest.TokenSymbol, opt => opt.MapFrom(src => src.TokenSymbol))
                .ForMember(dest => dest.TokenDecimals, opt => opt.MapFrom(src => src.TokenDecimals))
                .ForMember(dest => dest.TokenTotalSupply, opt => opt.MapFrom(src => src.TokenTotalSupply.ToString()))
                .ForMember(dest => dest.DeploymentTranactionHash, opt => opt.MapFrom(src => src.TransactionHash))
                .ForMember(dest => dest.DeploymentBlockHash, opt => opt.MapFrom(src => src.BlockHash))
                .AfterMap((erc20ContractModel, erc20ContractEntity) =>
                {
                    erc20ContractEntity.DeploymentBlockNumber = (ulong) erc20ContractModel.BlockNumber;
                    erc20ContractEntity.BlockTimestamp        = (uint)  erc20ContractModel.BlockTimestamp;
                })
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Erc20ContractEntity, Erc20ContractModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.DeployerAddress, opt => opt.MapFrom(src => src.DeployerAddress))
                .ForMember(dest => dest.TokenName, opt => opt.MapFrom(src => src.TokenName))
                .ForMember(dest => dest.TokenSymbol, opt => opt.MapFrom(src => src.TokenSymbol))
                .ForMember(dest => dest.TransactionHash, opt => opt.MapFrom(src => src.DeploymentTranactionHash))
                .ForMember(dest => dest.TokenDecimals, opt => opt.MapFrom(src => src.TokenDecimals))
                .ForMember(dest => dest.TokenTotalSupply, opt => opt.MapFrom(x => BigInteger.Parse(x.TokenTotalSupply)))
                .ForMember(dest => dest.BlockHash, opt => opt.MapFrom(src => src.DeploymentBlockHash))
                .AfterMap((erc20ContractEntity, erc20ContractModel) =>
                {
                    erc20ContractModel.BlockNumber    = erc20ContractEntity.DeploymentBlockNumber;
                    erc20ContractModel.BlockTimestamp = erc20ContractEntity.BlockTimestamp;
                })
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
