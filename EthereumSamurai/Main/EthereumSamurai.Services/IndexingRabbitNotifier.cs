﻿using EthereumSamurai.Core.Services;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.Models.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Services
{
    public class IndexingRabbitNotifier : IIndexingRabbitNotifier
    {
        private readonly IRabbitQueuePublisher _rabbitQueuePublisher;
        private readonly IIndexerInstanceSettings _indexerInstanceSettings;

        public IndexingRabbitNotifier(IRabbitQueuePublisher rabbitQueuePublisher, IIndexerInstanceSettings indexerInstanceSettings)
        {
            _rabbitQueuePublisher    = rabbitQueuePublisher;
            _indexerInstanceSettings = indexerInstanceSettings;
        }

        public async Task NotifyAsync(RabbitIndexingMessage rabbitIndexingMessage)
        {
            if (_indexerInstanceSettings.SendEventsToRabbit)
            {
                await _rabbitQueuePublisher.PublshEventAsync(rabbitIndexingMessage);
            }
        }
    }
}
