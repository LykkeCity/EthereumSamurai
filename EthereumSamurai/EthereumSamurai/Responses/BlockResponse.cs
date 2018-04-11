using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EthereumSamurai.Responses
{
    [DataContract]
    public class BlockResponse
    {
        [DataMember(Name = "blockHash")]
        public string BlockHash { get; set; }

        [DataMember(Name = "gasUsed")]
        public string GasUsed { get; set; }

        [DataMember(Name = "gasLimit")]
        public string GasLimit { get; set; }

        [DataMember(Name = "parentHash")]
        public string ParentHash { get; set; }

        [DataMember(Name = "number")]
        public ulong Number { get; set; }
    }
}
