using System.ComponentModel;

namespace Lykke.Job.EthereumSamurai.Messages
{
    [ImmutableObject(true)]
    public class IndexBlockBalancesMessage
    {
        public ulong BlockNumber { get; private set; }

        public IndexBlockBalancesMessage(ulong blockNumber)
        {
            BlockNumber = blockNumber;
        }
    }
}
