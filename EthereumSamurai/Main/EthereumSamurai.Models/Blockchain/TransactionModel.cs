using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace EthereumSamurai.Models.Blockchain
{
    public class TransactionModel
    {
        public string TransactionHash { get; set; }
        public BigInteger TransactionIndex { get; set; }
        public string BlockHash { get; set; }
        public BigInteger BlockNumber { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public BigInteger Gas { get; set; }
        public BigInteger GasPrice { get; set; }
        public BigInteger Value { get; set; }
        public string Input { get; set; }
        public BigInteger Nonce { get; set; }
    }
}
