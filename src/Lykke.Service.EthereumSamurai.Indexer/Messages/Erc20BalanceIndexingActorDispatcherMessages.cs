using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Messages.Erc20BalanceIndexingActorDispatcher
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
