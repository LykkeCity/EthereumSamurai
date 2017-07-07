using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Services
{
    public interface IDebug
    {
        Task TraceTransactionAsync(string transactionHash, bool withMemory, bool withStack, bool withStorage);
        Task TraceTransactionAsync(string fromAddress, string toAddress, BigInteger value,
            string transactionHash, bool withMemory, bool withStack, bool withStorage);
    }
}
