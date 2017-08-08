using System.Numerics;
using System.Threading.Tasks;
using EthereumSamurai.Models.DebugModels;

namespace EthereumSamurai.Core.Services
{
    public interface IDebug
    {
        Task<TraceResultModel> TraceTransactionAsync(string fromAddress, string toAddress, string contractAddress,
            BigInteger value, string transactionHash, bool withMemory, bool withStack, bool withStorage);
    }
}