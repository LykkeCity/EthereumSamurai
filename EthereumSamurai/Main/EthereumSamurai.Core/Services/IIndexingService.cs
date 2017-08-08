using System.Numerics;
using System.Threading.Tasks;
using EthereumSamurai.Models;

namespace EthereumSamurai.Core.Services
{
    public interface IIndexingService
    {
        Task<BigInteger> GetLastBlockAsync();

        Task<BigInteger?> GetLastBlockForIndexerAsync(string indexerId);

        Task IndexBlockAsync(BlockContext blockContext);
    }
}