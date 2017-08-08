using System.Threading.Tasks;
using EthereumSamurai.Models.Blockchain;

namespace EthereumSamurai.Core.Services
{
    public interface IBlockService
    {
        Task<bool> DoesBlockExist(string blockHash);

        Task<BlockModel> GetForHashAsync(string blockHash);
    }
}