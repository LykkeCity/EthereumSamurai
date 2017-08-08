using System.Numerics;
using System.Threading.Tasks;
using EthereumSamurai.Models;

namespace EthereumSamurai.Core.Services
{
    public interface IRpcBlockReader
    {
        Task<BigInteger> GetBlockCount();

        Task<BlockContent> ReadBlockAsync(BigInteger blockHeight);
    }
}