namespace Lykke.Service.EthereumSamurai.Core.Settings
{
    public class BaseSettings : IBaseSettings
    {
        public DB DB { get; set; }

        public string EthereumRpcUrl { get { return "http://104.40.144.206:8000"; } }
        public string ParityRpcUrl { get; set; }

        public RabbitMq RabbitMq { get; set; }
    }

    public class RabbitMq
    {
        public string Host { get; set; }
        public string ExternalHost { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ExchangeEthereumSamuraiBlocks { get; set; }
        public string ExchangeEthereumSamuraiErcTransfer { get; set; }
        public string ExchangeEthereumSamuraiErcContracts { get; set; }
        public string RoutingKey { get; set; }
    }
}