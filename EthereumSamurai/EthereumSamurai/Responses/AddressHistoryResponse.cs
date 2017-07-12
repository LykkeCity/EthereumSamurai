using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EthereumSamurai.Responses
{
    [DataContract]
    public class FilteredAddressHistoryResponse
    { 
        [DataMember(Name ="history")]
        public IEnumerable<AddressHistoryResponse> History { get; set; }
    }

    [DataContract]
    public class AddressHistoryResponse
    {
        [DataMember(Name = "blockNumber")]
        public ulong BlockNumber { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }

        [DataMember(Name = "transactionHash")]
        public string TransactionHash { get; set; }

        [DataMember(Name = "from")]
        public string From { get; set; }

        [DataMember(Name = "to")]
        public string To { get; set; }

        [DataMember(Name = "blockTimestamp")]
        public uint BlockTimestamp { get; set; }

        [DataMember(Name = "hasError")]
        public bool HasError { get; set; }

        [DataMember(Name = "transactionIndex")]
        public int TransactionIndex { get; set; }

        [DataMember(Name = "messageIndex")]
        public int MessageIndex { get; set; }
    }
}
