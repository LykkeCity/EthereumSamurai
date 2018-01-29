using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.EthereumSamurai.MongoDb.Entities
{
    public class Erc20ContractEntity
    {
        [BsonId]
        public string Address { get; set; }

        [BsonElement]
        public uint BlockTimestamp { get; set; }

        [BsonElement]
        public string DeployerAddress { get; set; }

        [BsonElement]
        public ulong DeploymentBlockNumber { get; set; }

        [BsonElement]
        public string DeploymentBlockHash { get; set; }

        [BsonElement]
        public string DeploymentTranactionHash { get; set; }

        [BsonElement]
        public uint? TokenDecimals { get; set; }

        [BsonElement]
        public string TokenName { get; set; }

        [BsonElement]
        public string TokenSymbol { get; set; }

        [BsonElement]
        public string TokenTotalSupply { get; set; }
    }
}
