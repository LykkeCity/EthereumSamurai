using System;
using System.Text;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Core.Settings;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Lykke.Service.EthereumSamurai.RabbitMQ
{
    public class Erc20IndexingQueue : IErc20ContractIndexingQueue
    {
        private const string Queue = "lykke.ethereum.indexer.ethereumsamurai.contracts";

        private readonly IModel           _channel;
        private readonly object           _dequeueLock;
        private readonly object           _enqueueLock;
        private readonly IBasicProperties _publishProperties;

        public Erc20IndexingQueue(IBaseSettings settings)
        {
            var connectionFactory = new ConnectionFactory
            {
                AutomaticRecoveryEnabled = true,
                HostName                 = settings.RabbitMq.ExternalHost,
                Port                     = settings.RabbitMq.Port,
                UserName                 = settings.RabbitMq.Username,
                Password                 = settings.RabbitMq.Password
            };

            _channel           = connectionFactory.CreateConnection().CreateModel();
            _dequeueLock       = new object();
            _enqueueLock       = new object();
            _publishProperties = _channel.CreateBasicProperties();

            _publishProperties.Persistent = true;

            _channel.QueueDeclare
            (
                queue:      Queue,
                durable:    true,
                exclusive:  false,
                autoDelete: false,
                arguments:  null
            );
        }

        public DeployedContractModel Dequeue()
        {
            lock (_dequeueLock)
            {
                var message = _channel.BasicGet(Queue, false);

                if (message != null)
                {
                    try
                    {
                        var payloadString = Encoding.UTF8.GetString(message.Body);
                        var contract      = JsonConvert.DeserializeObject<DeployedContractModel>(payloadString);
                        
                        _channel.BasicAck(message.DeliveryTag, false);

                        return contract;
                    }
                    catch (Exception e)
                    {
                        _channel.BasicNack(message.DeliveryTag, false, false);

                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public void Enqueue(DeployedContractModel contract)
        {
            lock (_enqueueLock)
            {
                var payloadString = JsonConvert.SerializeObject(contract);
                var payloadBytes  = Encoding.UTF8.GetBytes(payloadString);

                _channel.BasicPublish
                (
                    exchange:        string.Empty,
                    routingKey:      Queue,
                    basicProperties: _publishProperties,
                    body:            payloadBytes
                );
            }
        }
    }
}