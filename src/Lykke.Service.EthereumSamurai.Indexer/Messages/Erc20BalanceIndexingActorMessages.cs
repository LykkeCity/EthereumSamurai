using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Messages
{
    public partial class Erc20BalanceIndexingActor
    {
        public static IndexBlockMessage CreateIndexBlockMessage(ulong blockNumber)
        {
            return new Erc20BalanceIndexingActor.IndexBlockMessage(blockNumber);
        }

        [ImmutableObject(true)]
        public class IndexBlockMessage
        {
            public ulong BlockNumber { get; private set; }

            public IndexBlockMessage(ulong blockNumber)
            {
                BlockNumber = BlockNumber;
            }
        }
    }
}
