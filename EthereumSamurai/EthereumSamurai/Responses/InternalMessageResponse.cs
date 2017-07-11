using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EthereumSamurai.Responses
{
    [DataContract]
    public class FilteredInternalMessageResponse
    {
        [DataMember(Name ="messages")]
        public IEnumerable<InternalMessageResponse> Messages { get; set; }
    }

    [DataContract]
    public class InternalMessageResponse
    {
        [DataMember(Name = "transactionHash")]
        public string TransactionHash { get; set; }
        [DataMember(Name = "blockNumber")]
        public ulong BlockNumber { get; set; }
        [DataMember(Name = "fromAddress")]
        public string FromAddress { get; set; }
        [DataMember(Name = "toAddress")]
        public string ToAddress { get; set; }
        [DataMember(Name = "depth")]
        public int Depth { get; set; }
        [DataMember(Name = "value")]
        public string Value { get; set; }
        [DataMember(Name = "messageIndex")]
        public int MessageIndex { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
