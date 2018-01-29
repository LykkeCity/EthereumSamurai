using System.Runtime.Serialization;

namespace Lykke.Service.EthereumSamurai.Responses
{
    [DataContract]
    public class BalanceResponse
    {
        [DataMember(Name = "amount")]
        public string Amount { get; set; }
    }
}