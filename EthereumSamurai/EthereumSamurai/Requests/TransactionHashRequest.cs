using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace EthereumSamurai.Requests
{
    [DataContract]
    public class TransactionHashRequest
    {
        [FromRoute]
        [DataMember(Name = "transactionHash")]
        public string TransactionHash { get; set; }
    }
}