using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Lykke.Service.EthereumSamurai.Responses
{
    [DataContract]
    public class Erc20BalanceResponse
    {
        [DataMember(Name = "address")]
        public string AssetHolderAddress { get; set; }

        [DataMember(Name = "amount")]
        public string Balance { get; set; }

        [DataMember(Name = "blockNumber"), Required]
        public ulong BlockNumber { get; set; }

        [DataMember(Name = "contract")]
        public string ContractAddress { get; set; }
    }
}