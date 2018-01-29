using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Lykke.Service.EthereumSamurai.Models.Messages
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IndexingMessageType
    {
        Block       = 0,
        ErcBalances = 1,
        ErcContract = 2,
    }

    [DataContract]
    public class RabbitIndexingMessage
    {
        [DataMember]
        public ulong BlockNumber { get; set; }

        [DataMember]
        public IndexingMessageType IndexingMessageType { get; set; }
    }

    [DataContract]
    public class RabbitContractIndexingMessage : RabbitIndexingMessage
    {
        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string BlockHash { get; set; }

        [DataMember]
        public int BlockTimestamp { get; set; }

        [DataMember]
        public string DeployerAddress { get; set; }

        [DataMember]
        public uint? TokenDecimals { get; set; }

        [DataMember]
        public string TokenName { get; set; }

        [DataMember]
        public string TokenSymbol { get; set; }

        [DataMember]
        public string TokenTotalSupply { get; set; }

        [DataMember]
        public string TransactionHash { get; set; }

    }
}
