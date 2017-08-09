namespace EthereumSamurai.Core.Settings
{
    public class BaseSettings : IBaseSettings
    {
        public DB DB { get; set; }

        public string EthereumRpcUrl { get; set; }
    }
}