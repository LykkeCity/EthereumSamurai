﻿using EthereumSamurai.Core.Models;
using EthereumSamurai.Core.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.Indexer.Jobs
{
    public interface IBlockIndexingJobFactory
    {
        IJob GetJob(IIndexingSettings settings);
    }

    public class BlockIndexingJobFactory : IBlockIndexingJobFactory
    {
        private readonly IRpcBlockReader _rpcBlockReader;
        private readonly IIndexingService _indexingService;
        private readonly ILog _logger;
        private readonly IBlockService _blockService;

        public BlockIndexingJobFactory(IRpcBlockReader rpcBlockReader, IIndexingService indexingService, ILog logger, IBlockService blockService)
        {
            _logger = logger;
            _indexingService = indexingService;
            _rpcBlockReader = rpcBlockReader;
            _blockService = blockService;
        }

        public IJob GetJob(IIndexingSettings settings)
        {
            return new BlockIndexingJob(_rpcBlockReader, _indexingService, settings, _logger, _blockService);
        }
    }
}
