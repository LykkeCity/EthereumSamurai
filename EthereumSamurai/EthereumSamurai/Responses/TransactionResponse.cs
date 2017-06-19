using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EthereumSamurai.Responses
{
    [DataContract]
    public class FilteredTransactionsResponse
    {
        [DataMember(Name ="transactions")]
        public IEnumerable<TransactionResponse> Transactions { get; set; }
    }

    [DataContract]
    public class TransactionResponse
    {
        [DataMember(Name = "")]
        public int TransactionIndex { get; set; }
        [DataMember(Name = "")]
        public ulong BlockNumber { get; set; }
        [DataMember(Name = "")]
        public string Gas { get; set; }
        [DataMember(Name = "")]
        public string GasPrice { get; set; }
        [DataMember(Name = "")]
        public string Value { get; set; }
        [DataMember(Name = "")]
        public string Nonce { get; set; }
        [DataMember(Name = "")]
        public string TransactionHash { get; set; }
        [DataMember(Name = "")]
        public string BlockHash { get; set; }
        [DataMember(Name = "")]
        public string From { get; set; }
        [DataMember(Name = "")]
        public string To { get; set; }
        [DataMember(Name = "")]
        public string Input { get; set; }
        [DataMember(Name = "")]
        public int BlockTimestamp { get; set; }
    }
}
