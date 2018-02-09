using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Messages
{
    public static partial class Common
    {
        public static IndexedBlockNumberMessage CreateIndexedBlockNumberMessage(BigInteger indexedBlock, BigInteger nextBlock)
        {
            return new IndexedBlockNumberMessage(indexedBlock, nextBlock);
        }

        [ImmutableObject(true)]
        public sealed class IndexedBlockNumberMessage : IIndexedBlockNumberMessage
        {
            public IndexedBlockNumberMessage(BigInteger indexedBlock, BigInteger nextBlock)
            {
                IndexedBlock = indexedBlock;
                NextBlock = nextBlock;
            }

            public BigInteger IndexedBlock { get; private set; }
            public BigInteger NextBlock { get; private set; }
        }

        public interface IIndexedBlockNumberMessage
        {
            BigInteger IndexedBlock { get; }
            BigInteger NextBlock { get; }
        }
    }
}
