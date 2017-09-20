using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace EthereumSamurai.Models.Messages
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IndexingMessageType
    {
        Block = 0,
        ErcBalances = 1
    }

    [DataContract]
    public class RabbitIndexingMessage
    {
        [DataMember]
        public ulong BlockNumber { get; set; }

        [DataMember]
        public IndexingMessageType IndexingMessageType { get; set; }
    }
}
