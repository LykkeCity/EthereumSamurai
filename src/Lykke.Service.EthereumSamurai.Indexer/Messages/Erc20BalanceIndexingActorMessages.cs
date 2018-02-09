using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Messages.Erc20BalanceIndexingActor
{
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
