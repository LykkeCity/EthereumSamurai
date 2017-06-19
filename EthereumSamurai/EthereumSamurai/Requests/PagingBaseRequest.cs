using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EthereumSamurai.Requests
{
    [DataContract]
    public class PagingBaseRequest
    {
        [FromQuery]
        [DataMember(Name = "start")]
        public int Start { get; set; }

        [FromQuery]
        [DataMember(Name = "count")]
        public int Count { get; set; }
    }
}
