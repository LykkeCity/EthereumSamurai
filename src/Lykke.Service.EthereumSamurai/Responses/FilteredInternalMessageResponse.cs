using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lykke.Service.EthereumSamurai.Responses
{
    [DataContract]
    public class FilteredInternalMessageResponse
    {
        [DataMember(Name = "messages")]
        public IEnumerable<InternalMessageResponse> Messages { get; set; }
    }
}