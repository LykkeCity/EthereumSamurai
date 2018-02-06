namespace Lykke.Service.EthereumSamurai.Core.Settings
{
    public class DB
    {
        public string MongoDBConnectionString { get; set; }

        [Lykke.SettingsReader.Attributes.Optional]
        public string LogsConnectionString { get; set; }
    }
}