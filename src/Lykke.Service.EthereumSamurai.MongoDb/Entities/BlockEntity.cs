using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.EthereumSamurai.MongoDb.Entities
{
    public class BlockEntity
    {
        [BsonElement]
        public string BlockHash { get; set; }
        [BsonElement]
        public string GasUsed { get; set; }
        [BsonElement]
        public string GasLimit { get; set; }
        [BsonElement]
        public string Size { get; set; }
        [BsonElement]
        public string ExtraData { get; set; }
        [BsonElement]
        public string TotalDifficulty { get; set; }
        [BsonElement]
        public string Difficulty { get; set; }
        [BsonElement]
        public string Miner { get; set; }
        [BsonElement]
        public ulong Timestamp { get; set; }
        [BsonElement]
        public string ReceiptsRoot { get; set; }
        [BsonElement]
        public string TransactionsRoot { get; set; }
        [BsonElement]
        public string LogsBloom { get; set; }
        [BsonElement]
        public string Sha3Uncles { get; set; }
        [BsonElement]
        public string Nonce { get; set; }
        [BsonElement]
        public string ParentHash { get; set; }
        [BsonId]
        public ulong Number { get; set; }
        [BsonElement]
        public string StateRoot { get; set; }
        [BsonElement]
        public int TransactionsCount { get; set; }
    }
}
