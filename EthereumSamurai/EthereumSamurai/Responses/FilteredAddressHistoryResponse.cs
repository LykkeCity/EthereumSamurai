using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EthereumSamurai.Responses
{
    [DataContract]
    public class FilteredAddressHistoryResponse
    {
        [DataMember(Name = "history")]
        public IEnumerable<AddressHistoryResponse> History { get; set; }
    }
}