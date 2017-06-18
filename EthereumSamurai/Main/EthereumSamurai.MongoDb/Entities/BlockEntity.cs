using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.MongoDb.Entities
{
    public class BlockEntity
    {
        public string GasUsed { get; set; }
        public string GasLimit { get; set; }
        public string Size { get; set; }
        public string ExtraData { get; set; }
        public string TotalDifficulty { get; set; }
        public string Difficulty { get; set; }
        public string Miner { get; set; }
        public string Timestamp { get; set; }
        public string ReceiptsRoot { get; set; }
        public string TransactionsRoot { get; set; }
        public string LogsBloom { get; set; }
        public string Sha3Uncles { get; set; }
        public string Nonce { get; set; }
        public string ParentHash { get; set; }
        public string BlockHash { get; set; }
        public string Number { get; set; }
        public string StateRoot { get; set; }
        public int TransactionsCount { get; set; }
    }
}
