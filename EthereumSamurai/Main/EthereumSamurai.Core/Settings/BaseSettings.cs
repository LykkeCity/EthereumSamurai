using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.Core.Settings
{
    public interface IBaseSettings
    {
        DB Db { get; set; }
        string EthereumRpcUrl { get; set; }
    }

    public class BaseSettings : IBaseSettings
    {
        public DB Db { get; set; }
        public string EthereumRpcUrl { get; set; }
    }

    public class SettingsWrapper
    {
        public BaseSettings EthereumIndexer { get; set; }
    }

    public class DB
    {
        public string MongoDBConnectionString { get; set; }
    }

    public interface IIndexerInstanceSettings
    {
        string IndexerId { get; set; }
        int ThreadAmount { get; set; }
        ulong StartBlock { get; set; }
        ulong? StopBlock { get; set; }
    }

    public class IndexerInstanceSettings : IIndexerInstanceSettings
    {
        public string IndexerId { get; set; }
        public int ThreadAmount { get; set; }
        public ulong StartBlock { get; set; }
        public ulong? StopBlock { get; set; }
    }
}
