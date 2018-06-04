using Lykke.Job.EthereumSamurai.Messages.Delivery;
using System.ComponentModel;
using System.Numerics;

namespace Lykke.Job.EthereumSamurai.Messages.Common
{
    [ImmutableObject(true)]
    public sealed class IndexedBlockNumberMessage : IIndexedBlockNumberMessage, IDeliverable
    {
        public IndexedBlockNumberMessage(BigInteger indexedBlock, BigInteger nextBlock)
        {
            IndexedBlock = indexedBlock;
            NextBlock = nextBlock;
        }

        public BigInteger IndexedBlock { get; private set; }
        public BigInteger NextBlock { get; private set; }
        public long DiliveryId
        {
            get { return (long) IndexedBlock % long.MaxValue; }
        }
    }

    public interface IIndexedBlockNumberMessage
    {
        BigInteger IndexedBlock { get; }
        BigInteger NextBlock { get; }
    }
}
