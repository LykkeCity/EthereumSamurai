using System.Runtime.Serialization;

namespace Lykke.Service.EthereumSamurai.Responses
{
    [DataContract]
    public class InternalMessageResponse
    {
        [DataMember(Name = "blockNumber")]
        public ulong BlockNumber { get; set; }

        [DataMember(Name = "blockTimeStamp")]
        public uint BlockTimeStamp { get; set; }

        [DataMember(Name = "depth")]
        public int Depth { get; set; }

        [DataMember(Name = "fromAddress")]
        public string FromAddress { get; set; }

        [DataMember(Name = "messageIndex")]
        public int MessageIndex { get; set; }

        [DataMember(Name = "toAddress")]
        public string ToAddress { get; set; }

        [DataMember(Name = "transactionHash")]
        public string TransactionHash { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}