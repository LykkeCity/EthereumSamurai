using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.MongoDb.Entities
{
    public class Erc20ContractEntity
    {
        [BsonId]
        public string Address { get; set; }
        [BsonElement]
        public string TokenName { get; set; }
        [BsonElement]
        public string DeployerAddress { get; set; }
        [BsonElement]
        public ulong DeploymentBlockNumber { get; set; }
        [BsonElement]
        public string DeploymentBlockHash { get; set; }
        [BsonElement]
        public string DeploymentTranactionHash { get; set; }
        [BsonElement]
        public uint BlockTimestamp { get; set; }
    }
}
