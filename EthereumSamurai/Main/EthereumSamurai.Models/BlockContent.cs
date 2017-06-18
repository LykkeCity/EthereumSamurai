using EthereumSamurai.Models.Blockchain;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.Models
{
    public class BlockContent
    {
        public BlockModel BlockModel { get; set; }
        public List<TransactionModel> Transactions { get; set; }
    }
}
