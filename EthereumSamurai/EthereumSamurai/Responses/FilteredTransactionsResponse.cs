using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EthereumSamurai.Responses
{
    [DataContract]
    public class FilteredTransactionsResponse
    {
        [DataMember(Name = "transactions")]
        public IEnumerable<TransactionResponse> Transactions { get; set; }
    }
}