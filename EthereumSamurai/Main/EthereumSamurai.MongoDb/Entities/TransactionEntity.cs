using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.MongoDb.Entities
{
    public class TransactionEntity
    {
        public string TransactionHash { get; set; }
        public string TransactionIndex { get; set; }
        public string BlockHash { get; set; }
        public string BlockNumber { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Gas { get; set; }
        public string GasPrice { get; set; }
        public string Value { get; set; }
        public string Input { get; set; }
        public string Nonce { get; set; }
    }
}
