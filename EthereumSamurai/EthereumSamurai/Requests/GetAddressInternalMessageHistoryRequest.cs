using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EthereumSamurai.Requests
{
    [DataContract]
    public class GetAddressInternalMessageHistoryRequest : PagingBaseRequest
    {
        [FromRoute]
        [Required]
        [DataMember(Name ="address")]
        public string address { get; set; }

        [FromQuery]
        [DataMember(Name = "startBlock")]
        public ulong? StartBlock { get; set; }

        [FromQuery]
        [DataMember(Name = "stopBlock")]
        public ulong? StopBlock { get; set; }
    }
}
