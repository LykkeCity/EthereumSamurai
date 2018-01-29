using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models;

namespace Lykke.Service.EthereumSamurai.Core.Services
{
    public interface IRpcBlockReader
    {
        Task<BigInteger> GetBlockCount();

        Task<BlockContent> ReadBlockAsync(BigInteger blockHeight);
    }
}