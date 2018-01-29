using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lykke.Service.EthereumSamurai.Requests
{
    [DataContract]
    public class GetErc20BalanceRequest
    {
        [DataMember(Name = "assetHolder")]
        public string AssetHolder { get; set; }

        [DataMember(Name = "blockNumber")]
        public ulong? BlockNumber { get; set; }

        [DataMember(Name = "contracts")]
        public IEnumerable<string> Contracts { get; set; }
    }
}