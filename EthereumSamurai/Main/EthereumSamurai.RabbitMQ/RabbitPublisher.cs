using Common;
using EthereumSamurai.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EthereumSamurai.Models.Messages;

namespace EthereumSamurai.RabbitMQ
{
    public class RabbitQueuePublisher : IRabbitQueuePublisher
    {
        private IMessageProducer<string> _publisher;

        public RabbitQueuePublisher(IMessageProducer<string> publisher)
        {
            _publisher = publisher;
        }

        public async Task PublshEventAsync(string rabbitEvent)
        {
            await _publisher.ProduceAsync(rabbitEvent);
        }

        public async Task PublshEventAsync(RabbitIndexingMessage rabbitEvent)
        {
            string message = Newtonsoft.Json.JsonConvert.SerializeObject(rabbitEvent);
            await _publisher.ProduceAsync(message);
        }
    }
}
