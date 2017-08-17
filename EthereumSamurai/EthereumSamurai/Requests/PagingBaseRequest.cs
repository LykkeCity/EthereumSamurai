using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace EthereumSamurai.Requests
{
    [DataContract]
    public class PagingBaseRequest
    {
        [FromQuery]
        [DataMember(Name = "count")]
        public int Count { get; set; }

        [FromQuery]
        [DataMember(Name = "start")]
        public int Start { get; set; }
    }
}