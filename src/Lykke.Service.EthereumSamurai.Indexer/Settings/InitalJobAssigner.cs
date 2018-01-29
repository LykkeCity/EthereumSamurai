using System.Collections.Generic;
using Lykke.Service.EthereumSamurai.Core.Models;
using Lykke.Service.EthereumSamurai.Core.Repositories;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Core.Settings;
using Lykke.Service.EthereumSamurai.Indexer.Jobs;
using Common.Log;

namespace Lykke.Service.EthereumSamurai.Indexer.Settings
{
    public interface IInitalJobAssigner
    {
        IEnumerable<IJob> GetJobs();
    }

    public class InitalJobAssigner : IInitalJobAssigner
    {
        private readonly IErc20BalanceIndexingJobFactory  _erc20BalanceIndexingJobFactory;
        private readonly IErc20ContractIndexingJobFactory _erc20ContractIndexingJobFactory;
        private readonly IBlockIndexingJobFactory         _blockIndexingFactory;
        private readonly IBlockRepository                 _blockRepository;
        private readonly IIndexerInstanceSettings         _indexerInstanceSettings;
        private readonly ILog                             _logger;
        private readonly IRpcBlockReader                  _rpcBlockReader;

        public InitalJobAssigner(
            IBlockIndexingJobFactory         blockIndexingFactory,
            IBlockRepository                 blockRepository,
            IErc20BalanceIndexingJobFactory  erc20BalanceIndexingJobFactory,
            IErc20ContractIndexingJobFactory erc20ContractIndexingJobFactory,
            IIndexerInstanceSettings         indexerInstanceSettings,
            ILog                             logger,
            IRpcBlockReader                  rpcBlockReader)
        {
            _blockIndexingFactory            = blockIndexingFactory;
            _blockRepository                 = blockRepository;
            _erc20BalanceIndexingJobFactory  = erc20BalanceIndexingJobFactory;
            _erc20ContractIndexingJobFactory = erc20ContractIndexingJobFactory;
            _indexerInstanceSettings         = indexerInstanceSettings;
            _logger                          = logger;
            _rpcBlockReader                  = rpcBlockReader;
        }

        public IEnumerable<IJob> GetJobs()
        {
            var jobs = new List<IJob>();
            
            // Blocks indexers
            if (_indexerInstanceSettings.IndexBlocks)
            {
                var lastRpcBlock = (ulong) _rpcBlockReader.GetBlockCount().Result;
                var from         = _indexerInstanceSettings.StartBlock;
                var to           = _indexerInstanceSettings.StopBlock ?? lastRpcBlock;
                var partSize     = (to - from) / (ulong) _indexerInstanceSettings.ThreadAmount;

                ulong? toBlock = from;
                
                
                for (var i = 0; i < _indexerInstanceSettings.ThreadAmount; i++)
                {
                    var fromBlock = (ulong) toBlock + 1;

                    toBlock = fromBlock + partSize;
                    toBlock = toBlock < to ? toBlock : _indexerInstanceSettings.StopBlock;

                    var indexerId = $"{_indexerInstanceSettings.IndexerId}_thread_{i}";
                    var job       = _blockIndexingFactory.GetJob(new IndexingSettings
                    {
                        IndexerId = indexerId,
                        From      = fromBlock,
                        To        = toBlock
                    });

                    jobs.Add(job);
                }
            }
            
            // Balances indexer
            if (_indexerInstanceSettings.IndexBalances)
            {
                jobs.Add(_erc20BalanceIndexingJobFactory.GetJob(_indexerInstanceSettings.BalancesStartBlock));
            }
            
            // Contracts indexer
            if (_indexerInstanceSettings.IndexContracts)
            {
                for (var i = 0; i < _indexerInstanceSettings.ContractsIndexerThreadAmount; i++)
                {
                    var job = _erc20ContractIndexingJobFactory.GetJob();

                    jobs.Add(job);
                }
            }

            return jobs;
        }
    }
}