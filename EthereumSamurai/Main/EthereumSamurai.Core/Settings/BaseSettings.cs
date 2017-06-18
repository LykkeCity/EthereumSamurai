using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.Core.Settings
{
    public interface IBaseSettings
    {
        DB Db { get; set; }
        string EthereumRpcUrl {get;set;}
    }

    public class BaseSettings : IBaseSettings
    {
        public DB Db { get; set; }
        public string EthereumRpcUrl { get; set; }
    }

    public class SettingsWrapper
    {
        public IBaseSettings EthereumIndexerSettings { get; set; }
    }

    public class DB
    {
        public string MongoDBConnectionString { get; set; }
    }
}
