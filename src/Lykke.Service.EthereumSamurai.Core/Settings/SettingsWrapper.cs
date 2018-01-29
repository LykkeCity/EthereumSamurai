using Lykke.Service.EthereumSamurai.Settings.SlackNotifications;

namespace Lykke.Service.EthereumSamurai.Core.Settings
{
    public class AppSettings
    {
        public BaseSettings EthereumIndexer { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}