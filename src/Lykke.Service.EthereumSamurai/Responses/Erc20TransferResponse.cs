using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Runtime.Serialization;

namespace Lykke.Service.EthereumSamurai.Responses
{
    [DataContract]
    public class Erc20TransferHistoryResponse
    {
        [DataMember(Name = "blockHash")]
        public string BlockHash { get; set; }

        [DataMember(Name = "blockNumber"), Required]
        public ulong BlockNumber { get; set; }

        [DataMember(Name = "blockTimestamp"), Required]
        public ulong BlockTimestamp { get; set; }

        [DataMember(Name = "contract")]
        public string ContractAddress { get; set; }

        [DataMember(Name = "from")]
        public string From { get; set; }

        [DataMember(Name = "logIndex"), Required]
        public uint LogIndex { get; set; }

        [DataMember(Name = "to")]
        public string To { get; set; }

        [DataMember(Name = "transactionHash")]
        public string TransactionHash { get; set; }

        [DataMember(Name = "transactionIndex"), Required]
        public uint TransactionIndex { get; set; }

        [DataMember(Name = "transferAmount")]
        public string TransferAmount { get; set; }

        [DataMember(Name = "gasUsed")]
        public BigInteger GasUsed { get; internal set; }

        [DataMember(Name = "gasPrice")]
        public BigInteger GasPrice { get; internal set; }
    }
}