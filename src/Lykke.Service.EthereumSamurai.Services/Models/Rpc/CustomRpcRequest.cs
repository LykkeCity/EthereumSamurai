using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Lykke.Service.EthereumSamurai.Services.Models.Rpc
{
    [DataContract]
    public class CustomRpcRequest
    {
        public CustomRpcRequest()
        {
        }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "method")]
        public string Method { get; set; }

        [DataMember(Name = "params")]
        public List<object> Params { get; set; }

        [DataMember(Name = "jsonrpc")]
        public string JsonRpc
        {
            get
            {
                return "2.0";
            }
        }
    }
}
