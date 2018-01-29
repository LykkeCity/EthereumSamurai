using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.EthereumSamurai.MongoDb.Entities
{
    public class TransactionEntity
    {
        [BsonId]
        public string TransactionHash { get; set; }
        [BsonElement]
        public string TransactionIndex { get; set; }
        [BsonElement]
        public string BlockHash { get; set; }
        [BsonElement]
        public ulong BlockNumber { get; set; }
        [BsonElement]
        public string From { get; set; }
        [BsonElement]
        public string To { get; set; }
        [BsonElement]
        public string Gas { get; set; }
        [BsonElement]
        public string GasPrice { get; set; }
        [BsonElement]
        public string Value { get; set; }
        [BsonElement]
        public string Input { get; set; }
        [BsonElement]
        public string Nonce { get; set; }
        [BsonElement]
        public uint BlockTimestamp { get; set; }
        [BsonElement]
        public string ContractAddress { get; set; }
        [BsonElement]
        public string GasUsed { get; set; }
        [BsonElement]
        public string HasError { get; set; }
    }
}
