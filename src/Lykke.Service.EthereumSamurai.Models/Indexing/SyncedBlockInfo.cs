using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.EthereumSamurai.Models.Indexing
{
    [Obsolete]
    public class BlockSyncedInfoModel
    {
        public BlockSyncedInfoModel(string indexerId, ulong number)
        {
            this.IndexerId = indexerId;
            this.BlockNumber = number;
        }

        public string IndexerId { get; set; }
        public ulong BlockNumber { get; set; }
    }
}
