namespace EthereumSamurai.Core.Settings
{
    public class BaseSettings : IBaseSettings
    {
        public DbSettings DbSettings { get; set; }

        public string EthereumRpcUrl { get; set; }
    }
}