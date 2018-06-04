using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using Lykke.Job.EthereumSamurai.Messages.Delivery;

namespace Lykke.Job.EthereumSamurai.Messages
{
    [ImmutableObject(true)]
    public class IndexBlockMessage : IDeliverable
    {
        public BigInteger BlockNumber { get; private set; }

        public IndexBlockMessage(BigInteger blockNumber)
        {
            BlockNumber = blockNumber;
        }

        public long DiliveryId
        {
            get { return (long)BlockNumber % long.MaxValue; }
        }
    }
}
