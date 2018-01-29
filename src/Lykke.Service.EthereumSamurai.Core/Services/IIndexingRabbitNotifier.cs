using Lykke.Service.EthereumSamurai.Models.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.Core.Services
{
    public interface IIndexingRabbitNotifier
    {
        Task NotifyAsync(RabbitIndexingMessage rabbitIndexingMessage);
    }
}
