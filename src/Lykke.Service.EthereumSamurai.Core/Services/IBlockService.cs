using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models.Blockchain;

namespace Lykke.Service.EthereumSamurai.Core.Services
{
    public interface IBlockService
    {
        Task<bool> DoesBlockExist(string blockHash);

        Task<BlockModel> GetForHashAsync(string blockHash);
    }
}