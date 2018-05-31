using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.EthereumSamurai.MongoDb.Entities
{
    [Obsolete]
    public class BlockSyncedInfoEntity
    {
        [BsonId]
        public ulong BlockNumber { get; set; }
    }
}
