using MongoDB.Bson.Serialization.Attributes;

namespace Lykke.Service.EthereumSamurai.MongoDb.Entities
{
    public class BlockIndexationHistoryEntity
    {
        [BsonElement]
        public int? BalancesIndexerVersion { get; set; }

        [BsonId]
        public ulong BlockNumber { get; set; }

        [BsonElement]
        public int BlockIndexerVersion { get; set; }
        
        [BsonElement]
        public long Timestamp { get; set; }
    }
}