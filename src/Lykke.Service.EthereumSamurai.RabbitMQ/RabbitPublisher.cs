using Common;
using Lykke.Service.EthereumSamurai.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Models.Messages;
using RabbitMQ.Client;
using Lykke.Service.EthereumSamurai.Core.Settings;
using RabbitMQ.Client.Exceptions;

namespace Lykke.Service.EthereumSamurai.RabbitMQ
{
    public class RabbitQueuePublisher : IRabbitQueuePublisher
    {
        private readonly IBaseSettings _settings;
        private readonly IModel _channel;

        public RabbitQueuePublisher(IChannelFactory channelFactory, IBaseSettings settings)
        {
            _settings = settings;
            _channel = channelFactory.GetChannel();
            _channel.ExchangeDeclare(exchange: _settings.RabbitMq.ExchangeEthereumSamuraiErcContracts, type: "fanout", durable: true);
            _channel.ExchangeDeclare(exchange: _settings.RabbitMq.ExchangeEthereumSamuraiBlocks, type: "fanout", durable: true);
            _channel.ExchangeDeclare(exchange: _settings.RabbitMq.ExchangeEthereumSamuraiErcTransfer, type: "fanout", durable: true);
        }

        public async Task PublshEventAsync(RabbitIndexingMessage rabbitEvent)
        {
            var notificationJson = Newtonsoft.Json.JsonConvert.SerializeObject(rabbitEvent);
            var notification = Encoding.UTF8.GetBytes(notificationJson);

            try
            {
                string exchangeForEvent;
                
                switch (rabbitEvent.IndexingMessageType)
                {
                    case IndexingMessageType.Block:
                        exchangeForEvent = _settings.RabbitMq.ExchangeEthereumSamuraiBlocks;
                        break;

                    case IndexingMessageType.ErcBalances:
                        exchangeForEvent = _settings.RabbitMq.ExchangeEthereumSamuraiErcTransfer;
                        break;

                    case IndexingMessageType.ErcContract:
                        exchangeForEvent = _settings.RabbitMq.ExchangeEthereumSamuraiErcContracts;
                        break;

                    default:
                        throw new NotSupportedException("Such Event is not supported yet");
                }
                _channel.BasicPublish
                (
                    exchange: exchangeForEvent,
                    routingKey: _settings.RabbitMq.RoutingKey,
                    body: notification
                );
            }
            catch (AlreadyClosedException e) when (e.ShutdownReason?.ReplyCode == 404)
            {
                //TODO:
                throw;
            }
        }
    }
}