using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Services
{
    public interface IBlockService
    {
        Task<bool> DoesBlockExist(string blockHash);
        Task<BlockModel> GetForHashAsync(string blockHash);
    }
}
