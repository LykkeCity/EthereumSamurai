using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models;

namespace Lykke.Service.EthereumSamurai.Core.Services
{
    public interface IIndexingService
    {
        Task<BigInteger> GetLastBlockAsync();

        Task<BigInteger?> GetLastBlockForIndexerAsync(string indexerId);

        Task IndexBlockAsync(BlockContext blockContext);
    }
}