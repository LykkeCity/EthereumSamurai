using MongoDB.Bson.Serialization.Attributes;

namespace EthereumSamurai.MongoDb.Entities
{
    public class Erc20BalanceEntity
    {
        [BsonElement]
        public string AssetHolderAddress { get; set; }

        [BsonElement]
        public string Balance { get; set; }

        [BsonElement]
        public string ContractAddress { get; set; }
    }
}