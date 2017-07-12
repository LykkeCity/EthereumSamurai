using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.MongoDb.Entities
{
    public class AddressHistoryEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public ulong BlockNumber { get; set; }

        [BsonElement]
        public int TransactionIndex { get; set; }

        [BsonElement]
        public int MessageIndex { get; set; }

        [BsonElement]
        public string TransactionHash { get; set; }

        [BsonElement]
        public string Value { get; set; }

        [BsonElement]
        public string From { get; set; }

        [BsonElement]
        public string To { get; set; }

        [BsonElement]
        public uint BlockTimestamp { get; set; }

        [BsonElement]
        public bool HasError { get; set; }
    }
}
