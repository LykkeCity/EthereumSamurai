using System.ComponentModel;
using System.Numerics;

namespace Lykke.Job.EthereumSamurai.Messages
{
    [ImmutableObject(true)]
    public sealed class BalanceChangeMessage
    {
        public BigInteger BlockNumber { get; set; }
        public string AssetHolder { get; set; }
        public string ContractAddress { get; set; }
        public BigInteger Amount { get; set; }

        public BalanceChangeMessage(BigInteger blockNumber, string assetHolder, string contractAddress, BigInteger amount)
        {
            BlockNumber = blockNumber;
            AssetHolder = assetHolder;
            ContractAddress = contractAddress;
            Amount = amount;
        }
    }
}
