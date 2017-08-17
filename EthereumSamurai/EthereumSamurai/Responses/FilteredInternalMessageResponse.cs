using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EthereumSamurai.Responses
{
    [DataContract]
    public class FilteredInternalMessageResponse
    {
        [DataMember(Name = "messages")]
        public IEnumerable<InternalMessageResponse> Messages { get; set; }
    }
}