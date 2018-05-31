using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Messages
{
    public partial class BlockIndexingActor
    {
        public static IndexBlockMessage CreateIndexBlockMessage(BigInteger blockNumber)
        {
            return new IndexBlockMessage(blockNumber);
        }

        [ImmutableObject(true)]
        public class IndexBlockMessage
        {
            public BigInteger BlockNumber { get; private set; }

            public IndexBlockMessage(BigInteger blockNumber)
            {
                BlockNumber = blockNumber;
            }
        }
    }
}
