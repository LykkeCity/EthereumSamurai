using MongoDB.Bson.Serialization.Attributes;

namespace Lykke.Service.EthereumSamurai.MongoDb.Entities
{
    [BsonIgnoreExtraElements]
    public class Erc20BalanceEntity
    {
        [BsonElement]
        public string AssetHolderAddress { get; set; }

        [BsonElement]
        public string Balance { get; set; }

        [BsonElement]
        public ulong BlockNumber { get; set; }

        [BsonElement]
        public string ContractAddress { get; set; }
    }
}