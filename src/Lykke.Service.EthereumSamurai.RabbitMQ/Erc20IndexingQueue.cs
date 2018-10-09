using System;
using System.Text;
using System.Threading;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Core.Settings;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Lykke.Service.EthereumSamurai.RabbitMQ
{
    public class Erc20IndexingQueue : IErc20ContractIndexingQueue
    {
        private const string Queue = "lykke.ethereum.indexer.ethereumsamurai.contracts";

        private IModel _channel;
        private IBasicProperties _publishProperties;
        private readonly object _dequeueLock;
        private readonly object _enqueueLock;
        private readonly IBaseSettings _settings;
        private readonly ReaderWriterLockSlim _queueUpdateLock;
        private DateTime _lastTimeQueueWasCreated;

        public Erc20IndexingQueue(IBaseSettings settings)
        {
            _settings = settings;
            _dequeueLock = new object();
            _enqueueLock = new object();
            _queueUpdateLock = new ReaderWriterLockSlim();
            _lastTimeQueueWasCreated = DateTime.MinValue;

            CreateQueue();
        }

        public DeployedContractModel Dequeue()
        {
            lock (_dequeueLock)
            {
                BasicGetResult message = null;

                try
                {
                    message = _channel.BasicGet(Queue, false);
                }
                catch (OperationInterruptedException e)
                {
                    if (e.ShutdownReason.ReplyCode == 404)
                    {
                        CreateQueue();
                    }

                    throw;
                }

                if (message != null)
                {
                    try
                    {
                        var payloadString = Encoding.UTF8.GetString(message.Body);
                        var contract = JsonConvert.DeserializeObject<DeployedContractModel>(payloadString);

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
                try
                {
                    var payloadString = JsonConvert.SerializeObject(contract);
                    var payloadBytes = Encoding.UTF8.GetBytes(payloadString);

                    _queueUpdateLock.EnterReadLock();

                    _channel.BasicPublish
                    (
                        exchange: string.Empty,
                        routingKey: Queue,
                        basicProperties: _publishProperties,
                        body: payloadBytes
                    );
                }
                finally
                {
                    _queueUpdateLock.ExitReadLock();
                }
            }
        }

        private int _retryCount = 0;
        public void CreateQueue()
        {
            try
            {
                _retryCount++;
                _queueUpdateLock.EnterWriteLock();

                if (DateTime.UtcNow - _lastTimeQueueWasCreated < TimeSpan.FromMinutes(5))
                {
                    return;
                }

                var connectionFactory = new ConnectionFactory
                {
                    AutomaticRecoveryEnabled = true,
                    HostName = _settings.RabbitMq.ExternalHost,
                    Port = _settings.RabbitMq.Port,
                    UserName = _settings.RabbitMq.Username,
                    Password = _settings.RabbitMq.Password
                };

                _channel = connectionFactory.CreateConnection().CreateModel();
                _publishProperties = _channel.CreateBasicProperties();

                _publishProperties.Persistent = true;
                _channel.QueueDeclare
                (
                    queue: Queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                _lastTimeQueueWasCreated = DateTime.UtcNow;
            }
            catch (BrokerUnreachableException e)
            {
                if (_retryCount >= 5)
                {
                    throw;
                }
                
                _queueUpdateLock.ExitWriteLock();
                Thread.Sleep(5000);
                CreateQueue();
            }
            finally
            {
                _queueUpdateLock.ExitWriteLock();
            }
        }
    }
}