using System.Runtime.Serialization;

namespace EthereumSamurai.Responses
{
    [DataContract]
    public class BalanceResponse
    {
        [DataMember(Name = "amount")]
        public string Amount { get; set; }
    }
}