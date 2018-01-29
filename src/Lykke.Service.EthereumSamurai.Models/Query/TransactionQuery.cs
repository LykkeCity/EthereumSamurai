using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.EthereumSamurai.Models.Query
{
    public class TransactionQuery
    {
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public int? Start { get; set; }
        public int? Count { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
