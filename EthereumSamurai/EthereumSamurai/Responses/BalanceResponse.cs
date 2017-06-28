using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EthereumSamurai.Responses
{
    [DataContract]
    public class BalanceResponse
    {
        [DataMember(Name = "amount")]
        public string Amount { get; set; }
    }
}
