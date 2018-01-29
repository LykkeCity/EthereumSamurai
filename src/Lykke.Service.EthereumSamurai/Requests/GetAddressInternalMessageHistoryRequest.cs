using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumSamurai.Requests
{
    [DataContract]
    public class GetAddressInternalMessageHistoryRequest : PagingBaseRequest
    {
        [FromRoute]
        [Required]
        [DataMember(Name = "address")]
        public string Address { get; set; }

        [FromQuery]
        [DataMember(Name = "startBlock")]
        public ulong? StartBlock { get; set; }

        [FromQuery]
        [DataMember(Name = "stopBlock")]
        public ulong? StopBlock { get; set; }
    }
}