using System.Runtime.Serialization;

namespace EthereumSamurai.Responses
{
    [DataContract]
    public class Erc20BalanceResponse
    {
        [DataMember(Name = "address")]
        public string AssetHolderAddress { get; set; }

        [DataMember(Name = "amount")]
        public string Balance { get; set; }

        [DataMember(Name = "contract")]
        public string ContractAddress { get; set; }
    }
}