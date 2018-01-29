using Lykke.Service.EthereumSamurai.Core.Settings;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.EthereumSamurai.RabbitMQ
{
    public interface IChannelFactory
    {
        IModel GetChannel();
    }

    public class ChannelFactory : IChannelFactory
    {
        private readonly IBaseSettings _settings;

        public ChannelFactory(IBaseSettings settings)
        {
            _settings = settings;
        }

        public IModel GetChannel()
        {
            string connectionStringName;

            var connectionString = $"amqp://{_settings.RabbitMq.Username}:{_settings.RabbitMq.Password}@{_settings.RabbitMq.ExternalHost}:{_settings.RabbitMq.Port}";
            var rabbitUri = new Uri(connectionString);

            var connectionFactory = new ConnectionFactory
            {
                AutomaticRecoveryEnabled = true,
                Uri = rabbitUri.ToString()
            };

            var connection = connectionFactory.CreateConnection();

            return connection.CreateModel();
        }
    }
}
