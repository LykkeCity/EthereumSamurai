using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Lykke.Service.EthereumSamurai.Requests
{
    [DataContract]
    public class GetErc20TransferHistoryRequest
    {
        [DataMember(Name = "assetHolder")]
        public string AssetHolder { get; set; }

        [DataMember(Name = "blockNumber")]
        public ulong? BlockNumber { get; set; }

        [DataMember(Name = "contracts")]
        public IEnumerable<string> Contracts { get; set; }
    }

    [DataContract]
    public class GetErc20TransferHistoryV2Request
    {
        [Required]
        [DataMember(Name = "assetHolder")]
        public string AssetHolder { get; set; }

        [DataMember(Name = "blockNumber")]
        public ulong? BlockNumber { get; set; }

        [DataMember(Name = "contractAddress")]
        public string ContractAddress { get; set; }
    }
}