using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.MongoDb.Entities
{
    public class BlockSyncedInfoEntity
    {
        [BsonId]
        public ulong BlockNumber { get; set; }
        
        [BsonElement]
        public string IndexerId { get; set; }
    }
}
