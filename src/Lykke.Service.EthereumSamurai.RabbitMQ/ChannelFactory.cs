using Lykke.Service.EthereumSamurai.Core.Settings;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RabbitMQ.Client.Exceptions;

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

        private int _retryCount = 0;
        public IModel GetChannel()
        {
            try
            {
                _retryCount++;
                string connectionStringName;

                var connectionString =
                    $"amqp://{_settings.RabbitMq.Username}:{_settings.RabbitMq.Password}@{_settings.RabbitMq.ExternalHost}:{_settings.RabbitMq.Port}";
                var rabbitUri = new Uri(connectionString);

                var connectionFactory = new ConnectionFactory
                {
                    AutomaticRecoveryEnabled = true,
                    Uri = rabbitUri.ToString()
                };

                var connection = connectionFactory.CreateConnection();

                return connection.CreateModel();
            }
            catch (BrokerUnreachableException e)
            {
                if (_retryCount >= 5)
                {
                    throw;
                }

                Thread.Sleep(5000);
                return GetChannel();
            }
        }
    }
}
