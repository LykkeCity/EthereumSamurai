using Common;
using EthereumSamurai.Core.Settings;
using Lykke.RabbitMqBroker.Publisher;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Lykke.RabbitMqBroker.Subscriber;
using RabbitMQ.Client;
using Common.Log;
using EthereumSamurai.Core.Services;
//using EthereumSamurai.Core.Services;

namespace EthereumSamurai.RabbitMQ
{
    public static class RegisterRabbitQueueEx
    {
        public static void RegisterRabbitQueue(this IServiceCollection services, IBaseSettings settings, ILog logger, string exchangePrefix = "")
        {
            string exchangeName = exchangePrefix + settings.RabbitMq.ExchangeEthereumSamurai;
            RabbitMqSubscriptionSettings rabbitMqSettings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = $"amqp://{settings.RabbitMq.Username}:{settings.RabbitMq.Password}@{settings.RabbitMq.ExternalHost}:{settings.RabbitMq.Port}",
                ExchangeName = exchangeName
            };

            RabbitMqPublisher<string> publisher = new RabbitMqPublisher<string>(rabbitMqSettings)
                .SetSerializer(new BytesSerializer())
                .SetPublishStrategy(new PublishStrategy(settings.RabbitMq.RoutingKey))
                .SetLogger(logger)
                .Start();

            services.AddSingleton<IMessageProducer<string>>(publisher);
            services.AddSingleton<IRabbitQueuePublisher, RabbitQueuePublisher>();
        }
    }

    internal class PublishStrategy : IRabbitMqPublishStrategy
    {
        private readonly string _queue;

        public PublishStrategy(string queue)
        {
            _queue = queue;
        }

        public void Configure(RabbitMqSubscriptionSettings settings, IModel channel)
        {
            channel.ExchangeDeclare(exchange: settings.ExchangeName, type: ExchangeType.Fanout, durable: true);
        }

        public void Publish(RabbitMqSubscriptionSettings settings, IModel channel, byte[] data)
        {
            channel.BasicPublish(exchange: settings.ExchangeName,
                     routingKey: _queue,//remove
                     basicProperties: null,
                     body: data);
        }
    }
}
