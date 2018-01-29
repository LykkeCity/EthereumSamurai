using Lykke.Service.EthereumSamurai.Core.Settings.ServiceSettings;
using Lykke.Service.EthereumSamurai.Core.Settings.SlackNotifications;

namespace Lykke.Service.EthereumSamurai.Core.Settings
{
    public class AppSettings
    {
        public EthereumSamuraiSettings EthereumSamuraiService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
