using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace EthereumSamurai.Requests
{
    [DataContract]
    public class GetErc20TokenRequest : PagingBaseRequest
    {
        [FromQuery]
        [DataMember(Name = "query")]
        public string Query { get; set; }
    }
}