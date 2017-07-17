using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace EthereumSamurai.Services.Models.Rpc
{
    [DataContract]
    public class CustomRpcRequest
    {
        [DataMember(Name ="id")]
        public string Id { get; set; }
        [DataMember(Name = "method")]
        public string Method { get; set; }
        [DataMember(Name = "params")]
        public List<object> Params { get; set; }
    }
}
