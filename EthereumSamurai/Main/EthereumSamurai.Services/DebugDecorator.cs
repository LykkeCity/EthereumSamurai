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
using EthereumSamurai.Core.Settings;
using System.Net.Http;
using EthereumSamurai.Services.Models.Rpc;
using System.IO;

namespace EthereumSamurai.Services
{
    /*
     {"id":1,
     "jsonrpc":"2.0",
     "method":"debug_traceTransaction",
     "params":["0x3685d6a6c2c6b27c846fc54b48886e14b3cfde6101973466359474fc27982395",
     {
     "disableStorage":true,
     "disableMemory":true,
     "disableStack":false,
     "fullStorage":false
     }]
     }
         */

    public class DebugDecorator : IDebug
    {
        private readonly DebugApiService _debugApiService;
        private HttpClient _httpClient;
        private readonly IBaseSettings _settings;

        public DebugDecorator(DebugApiService debugApiService, IBaseSettings settings)
        {
            _settings = settings;
            _httpClient = new HttpClient();
            _debugApiService = debugApiService;
        }

        public async Task<TraceResultModel> TraceTransactionAsync(string fromAddress, string toAddress, string contractAddress, BigInteger value,
            string transactionHash, bool withMemory, bool withStack, bool withStorage)
        {
             //Newtonsoft.Json.Linq.JObject jObject =
             //   await _debugApiService.TraceTransaction.SendRequestAsync(transactionHash, new Nethereum.Geth.RPC.Debug.DTOs.TraceTransactionOptions()
             //   {
             //       DisableMemory = !withMemory,
             //       DisableStack = !withStack,
             //       DisableStorage = !withStorage
             //   });

            CustomRpcRequest request = new CustomRpcRequest()
            {
                Id = "1",
                Method = "debug_traceTransaction",
                Params = new List<object>()
                {
                    transactionHash,
                    new Nethereum.Geth.RPC.Debug.DTOs.TraceTransactionOptions()
                    {
                        DisableMemory = !withMemory,
                        DisableStack = !withStack,
                        DisableStorage = !withStorage
                    }
                }
            };
            byte[] byteArray = Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(request));
            MemoryStream stream = new MemoryStream(byteArray);
            HttpResponseMessage response = await _httpClient.PostAsync(_settings.EthereumRpcUrl, new StreamContent(stream));
            var responseString = await response.Content.ReadAsStringAsync();
            TransactionTraceResponse traceResponse = (TransactionTraceResponse)Newtonsoft.Json.JsonConvert.DeserializeObject(responseString, typeof(TransactionTraceResponse));
            TransactionTrace trace = traceResponse.TransactionTrace;//jObject.ToObject<TransactionTrace>();//

            TransactionTracer tracer = new TransactionTracer(fromAddress, transactionHash, toAddress, contractAddress, value);

            foreach (var log in trace.StructLogs)
            {
                await tracer.ProcessLog(log);
            }

            TraceResult result = tracer.BuildResult();

            return new TraceResultModel()
            {
                HasError = result.HasError,
                Transfers = result.Transfers?.Where(x => x.Type != TransferValueType.TRANSACTION)
                .Select((x, counter) => new TransferValueModel()
                {
                    MessageIndex = counter,
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
