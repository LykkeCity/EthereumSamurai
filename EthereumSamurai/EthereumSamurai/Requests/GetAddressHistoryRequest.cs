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
    public class GetAddressHistoryRequest : PagingBaseRequest
    {
        [FromRoute]
        [Required]
        [DataMember(Name ="address")]
        public string Address { get; set; }
    }
}
