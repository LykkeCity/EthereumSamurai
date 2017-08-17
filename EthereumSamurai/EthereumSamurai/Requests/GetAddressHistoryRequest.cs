using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace EthereumSamurai.Requests
{
    [DataContract]
    public class GetAddressHistoryRequest : PagingBaseRequest
    {
        [FromRoute]
        [Required]
        [DataMember(Name = "address")]
        public string Address { get; set; }
    }
}