using EthereumSamurai.Models;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Services
{
    public interface IRpcBlockReader
    {
        Task<BlockContent> ReadBlockAsync(BigInteger blockHeight);
        Task<BigInteger> GetBlockCount();
    }
}
