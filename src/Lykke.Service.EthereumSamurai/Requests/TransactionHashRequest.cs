using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumSamurai.Requests
{
    [DataContract]
    public class TransactionHashRequest
    {
        [FromRoute]
        [DataMember(Name = "transactionHash")]
        public string TransactionHash { get; set; }
    }
}