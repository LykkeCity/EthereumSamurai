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
        public List<InternalMessageModel> InternalMessages { get; set; }
    }

    public class BlockContext
    {
        public BlockContext(string indexerId, BlockContent blockContent)
        {
            this.IndexerId = indexerId;
            this.BlockContent = blockContent;
        }

        public BlockContent BlockContent { get; private set; }
        public string IndexerId { get; private set; }
    }
}
