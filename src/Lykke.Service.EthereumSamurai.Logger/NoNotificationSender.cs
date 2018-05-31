using System.Threading.Tasks;
using Lykke.SlackNotifications;

namespace Lykke.Service.EthereumSamurai.Logger
{
    public class NoNotificationSender : ISlackNotificationsSender
    {
        public Task SendAsync(string type, string sender, string message)
        {
            return Task.FromResult(0);
        }
    }
}
