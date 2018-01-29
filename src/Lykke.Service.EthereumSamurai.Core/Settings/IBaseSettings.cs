namespace Lykke.Service.EthereumSamurai.Core.Settings
{
    public interface IBaseSettings
    {
        DB DB { get; set; }

        string EthereumRpcUrl { get; set; }
        string ParityRpcUrl { get; set; }

        RabbitMq RabbitMq { get; set; }
    }
}