using MongoDB.Bson.Serialization.Attributes;

namespace Lykke.Service.EthereumSamurai.MongoDb.Entities
{
    [BsonIgnoreExtraElements]
    public class Erc20TransferHistoryEntity
    {
        [BsonElement]
        public string BlockHash { get; set; }

        [BsonElement]
        public ulong BlockNumber { get; set; }

        [BsonElement]
        public ulong BlockTimestamp { get; set; }

        [BsonElement]
        public string ContractAddress { get; set; }

        [BsonElement]
        public string From { get; set; }
        
        [BsonElement]
        public uint LogIndex { get; set; }

        [BsonElement]
        public string To { get; set; }

        [BsonElement]
        public string TransactionHash { get; set; }

        [BsonElement]
        public uint TransactionIndex { get; set; }

        [BsonElement]
        public string TransferAmount { get; set; }

        [BsonElement]
        public string GasUsed { get; set; }

        [BsonElement]
        public string GasPrice { get; set; }
    }
}