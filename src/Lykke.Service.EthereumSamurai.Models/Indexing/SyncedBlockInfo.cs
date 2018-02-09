using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.EthereumSamurai.Models.Indexing
{
    [Obsolete]
    public class BlockSyncedInfoModel
    {
        public BlockSyncedInfoModel( ulong number)
        {
            this.BlockNumber = number;
        }

        public ulong BlockNumber { get; set; }
    }
}
