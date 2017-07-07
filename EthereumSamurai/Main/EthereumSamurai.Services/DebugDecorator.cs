using EthereumSamurai.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Nethereum.Contracts;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC;
using Nethereum.RPC.TransactionManagers;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Geth;
using Nethereum.Geth.RPC.Debug;
using System.Threading.Tasks;
using EthereumSamurai.Models.DebugModels;
using EthereumSamurai.Services.Models.DebugModels;
using System.Numerics;
using System.Linq;

namespace EthereumSamurai.Services
{
    public class DebugDecorator : IDebug
    {
        private readonly DebugApiService _debugApiService;

        public DebugDecorator(DebugApiService debugApiService)
        {
            _debugApiService = debugApiService;
        }

        public async Task<TraceResultModel> TraceTransactionAsync(string fromAddress, string toAddress, BigInteger value,
            string transactionHash, bool withMemory, bool withStack, bool withStorage)
        {
            Newtonsoft.Json.Linq.JObject jObject =
                await _debugApiService.TraceTransaction.SendRequestAsync(transactionHash, new Nethereum.Geth.RPC.Debug.DTOs.TraceTransactionOptions()
                {
                    DisableMemory = !withMemory,
                    DisableStack = !withStack,
                    DisableStorage = !withStack,
                    FullStorage = !withMemory && !withStack && !withStack
                });

            List<string> tokens = new List<string>(jObject.Count);
            var str = jObject.ToString();
            TransactionTrace trace = jObject.ToObject<TransactionTrace>();

            TransactionTracer tracer = new TransactionTracer(fromAddress, transactionHash, toAddress, value);

            foreach (var log in trace.StructLogs)
            {
                await tracer.ProcessLog(log);
            }

            TraceResult result = tracer.BuildResult();

            return new TraceResultModel()
            {
                HasError = result.HasError,
                Transfers = result.Transfers?.Select(x => new TransferValueModel()
                {
                    Depth = x.Depth,
                    FromAddress = x.FromAddress,
                    ToAddress = x.ToAddress,
                    TransactionHash = x.TransactionHash,
                    Type = (TransferValueModelType)x.Type,
                    Value = x.Value,
                })
            };
        }
    }
}
