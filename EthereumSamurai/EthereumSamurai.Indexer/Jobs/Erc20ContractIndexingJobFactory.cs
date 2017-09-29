﻿using Common.Log;
using EthereumSamurai.Core.Services;

namespace EthereumSamurai.Indexer.Jobs
{
    public class Erc20ContractIndexingJobFactory : IErc20ContractIndexingJobFactory
    {
        private readonly IErc20ContractIndexingService _indexingService;
        private readonly ILog                          _logger;


        public Erc20ContractIndexingJobFactory(
            IErc20ContractIndexingService indexingService,
            ILog logger)
        {
            _indexingService = indexingService;
            _logger          = logger;
        }

        public IJob GetJob()
        {
            return new Erc20ContractIndexingJob(_indexingService, _logger);
        }
    }
}