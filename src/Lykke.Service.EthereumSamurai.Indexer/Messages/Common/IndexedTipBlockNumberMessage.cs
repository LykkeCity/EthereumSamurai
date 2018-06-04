using System.ComponentModel;
using System.Numerics;

namespace Lykke.Job.EthereumSamurai.Messages.Common
{
    [ImmutableObject(true)]
    public sealed class IndexedTipBlockNumberMessage : IIndexedBlockNumberMessage
    {
        public IndexedTipBlockNumberMessage(BigInteger indexedBlock, BigInteger nextBlock)
        {
            IndexedBlock = indexedBlock;
            NextBlock = nextBlock;
        }

        public BigInteger IndexedBlock { get; private set; }
        public BigInteger NextBlock { get; private set; }
    }
}
