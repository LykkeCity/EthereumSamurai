using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Messages
{
    public static partial class Common
    {
        public static IndexedBlockNumberMessage CreateIndexedBlockNumberMessage(string indexerId, BigInteger indexedBlock, BigInteger nextBlock)
        {
            return new IndexedBlockNumberMessage(indexerId, indexedBlock, nextBlock);
        }

        public class IndexedBlockNumberMessage
        {
            public IndexedBlockNumberMessage(string indexerId, BigInteger indexedBlock, BigInteger nextBlock)
            {
                IndexedBlock = indexedBlock;
                NextBlock = nextBlock;
                IndexerId = indexerId;
            }

            public BigInteger IndexedBlock { get; private set; }
            public BigInteger NextBlock { get; private set; }
            public string IndexerId { get; private set; }
        }
    }
}
