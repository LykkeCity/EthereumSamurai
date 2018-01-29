using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.EthereumSamurai.Models.Query
{
    public class AddressHistoryQuery
    {
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public int? Start { get; set; }
        public int? Count { get; set; }
        public ulong? StartBlock { get; set; }
        public ulong? StopBlock { get; set; }
    }
}
