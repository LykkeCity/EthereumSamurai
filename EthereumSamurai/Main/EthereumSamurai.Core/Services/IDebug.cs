using EthereumSamurai.Models.DebugModels;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Services
{
    public interface IDebug
    {
        Task<TraceResultModel> TraceTransactionAsync(string fromAddress, string toAddress, BigInteger value,
            string transactionHash, bool withMemory, bool withStack, bool withStorage);
    }
}
