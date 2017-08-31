using EthereumSamurai.Models.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Services
{
    public interface IIndexingRabbitNotifier
    {
        Task NotifyAsync(RabbitIndexingMessage rabbitIndexingMessage);
    }
}
