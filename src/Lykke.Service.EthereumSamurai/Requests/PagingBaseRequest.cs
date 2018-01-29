using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumSamurai.Requests
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