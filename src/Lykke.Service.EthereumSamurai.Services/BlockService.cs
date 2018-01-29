using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.Services
{
    public class BlockService : IBlockService
    {
        private readonly IBlockRepository _blockRepository;

        public BlockService(IBlockRepository blockRepository)
        {
            _blockRepository = blockRepository;
        }

        public async Task<bool> DoesBlockExist(string blockHash)
        {
            bool exists= await _blockRepository.DoesBlockExistAsync(blockHash);

            return exists;
        }

        public async Task<BlockModel> GetForHashAsync(string blockHash)
        {
            BlockModel blockModel = await _blockRepository.GetForHashAsync(blockHash);

            return blockModel;
        }

        public async Task<BlockModel> GetForBlockNumberAsync(ulong blockNumber)
        {
            BlockModel blockModel = await _blockRepository.GetForNumberAsync(blockNumber);

            return blockModel;
        }
    }
}
