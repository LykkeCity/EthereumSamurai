using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Service.EthereumSamurai.Models.Blockchain
{
    public class BlockModel
    {
        public BigInteger GasUsed { get; set; }
        public BigInteger GasLimit { get; set; }
        public BigInteger Size { get; set; }
        public BigInteger TotalDifficulty { get; set; }
        public BigInteger Difficulty { get; set; }
        public BigInteger Timestamp { get; set; }
        public BigInteger Number { get; set; }
        public string ExtraData { get; set; }
        public string Miner { get; set; }
        public string ReceiptsRoot { get; set; }
        public string TransactionsRoot { get; set; }
        public string LogsBloom { get; set; }
        public string Sha3Uncles { get; set; }
        public string Nonce { get; set; }
        public string ParentHash { get; set; }
        public string BlockHash { get; set; }
        public string StateRoot { get; set; }
        public int TransactionsCount { get; set; }
    }
}
