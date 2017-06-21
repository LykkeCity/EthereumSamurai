using EthereumSamurai.Core.Models;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Core.Settings;
using EthereumSamurai.Indexer.Jobs;
using EthereumSamurai.MongoDb.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.Indexer.Settings
{
    public interface IInitalJobAssigner
    {
        IEnumerable<IJob> GetJobs();
    }

    public class InitalJobAssigner : IInitalJobAssigner
    {
        private readonly IIndexerInstanceSettings _indexerInstanceSettings;
        private readonly IBlockRepository _blockRepository;
        private readonly IRpcBlockReader _rpcBlockReader;
        private readonly IBlockIndexingJobFactory _factory;
        private readonly ILoggerFactory _loggerFactory;

        public InitalJobAssigner(
            IIndexerInstanceSettings indexerInstanceSettings,
            IBlockRepository         blockRepository,
            IRpcBlockReader          rpcBlockReader,
            IBlockIndexingJobFactory factory,
            ILoggerFactory           loggerFactory)
        {
            _indexerInstanceSettings = indexerInstanceSettings;
            _blockRepository = blockRepository;
            _rpcBlockReader = rpcBlockReader;
            _factory = factory;
            _loggerFactory = loggerFactory;
        }

        public IEnumerable<IJob> GetJobs()
        {
            List<IJob> jobs = new List<IJob>();

            ulong lastRpcBlock = (ulong)_rpcBlockReader.GetBlockCount().Result;
            ulong from = _indexerInstanceSettings.StartBlock;
            ulong to = !_indexerInstanceSettings.StopBlock.HasValue ? lastRpcBlock : _indexerInstanceSettings.StopBlock.Value;
            ulong partSize = (to - from) / (ulong)_indexerInstanceSettings.ThreadAmount;

            ulong fromBlock;
            ulong? toBlock = from;
            for (int i = 0; i < _indexerInstanceSettings.ThreadAmount; i++)
            {
                fromBlock = (ulong)toBlock + 1;
                toBlock = fromBlock + partSize;
                toBlock = toBlock < to ? toBlock : _indexerInstanceSettings.StopBlock;
                string indexerId = $"{_indexerInstanceSettings.IndexerId}_thread_{i}";
                IJob job = _factory.GetJob(new IndexingSettings()
                {
                    IndexerId = indexerId,
                    From = fromBlock,
                    To = toBlock,
                });

                jobs.Add(job);
            }

            return jobs;
        }
    }
}
