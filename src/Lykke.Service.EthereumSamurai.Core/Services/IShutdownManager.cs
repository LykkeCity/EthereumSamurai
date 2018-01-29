using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}