using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Service.EthereumSamurai.MongoDb.Entities
{
    public class InternalMessageEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement]
        public int MessageIndex { get; set; }
        [BsonElement]
        public string TransactionHash { get; set; }
        [BsonElement]
        public ulong BlockNumber { get; set; }
        [BsonElement]
        public string FromAddress { get; set; }
        [BsonElement]
        public string ToAddress { get; set; }
        [BsonElement]
        public string Value { get; set; }
        [BsonElement]
        public int Depth { get; set; }
        [BsonElement]
        public int Type { get; set; }
        [BsonElement]
        public uint BlockTimestamp { get; set; }
    }
}
