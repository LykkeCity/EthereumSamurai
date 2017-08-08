namespace EthereumSamurai.Core.Settings
{
    public interface IBaseSettings
    {
        DbSettings DbSettings { get; set; }

        string EthereumRpcUrl { get; set; }
    }
}