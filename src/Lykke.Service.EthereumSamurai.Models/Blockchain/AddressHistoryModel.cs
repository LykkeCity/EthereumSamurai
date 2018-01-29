using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Service.EthereumSamurai.Models.Blockchain
{
    public class AddressHistoryModel
    {
        public ulong BlockNumber { get; set; }

        public BigInteger Value { get; set; }

        public string TransactionHash { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public uint BlockTimestamp { get; set; }

        public bool HasError { get; set; }

        public int TransactionIndex { get; set; }

        public int MessageIndex { get; set; }
        public BigInteger GasUsed { get; set; }
        public BigInteger GasPrice { get; set; }
    }
}
