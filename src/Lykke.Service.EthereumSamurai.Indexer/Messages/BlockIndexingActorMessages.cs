using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Messages
{
    public partial class BlockIndexingActor
    {
        public static IndexBlockMessage CreateIndexBlockMessage(string indexerId, BigInteger blockNumber)
        {
            return new IndexBlockMessage(indexerId, blockNumber);
        }

        [ImmutableObject(true)]
        public class IndexBlockMessage
        {
            public string IndexerId { get; private set; }

            public BigInteger BlockNumber { get; private set; }

            public IndexBlockMessage(string indexerId, BigInteger blockNumber)
            {
                IndexerId = indexerId;
                BlockNumber = BlockNumber;
            }
        }
    }
}
