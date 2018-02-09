//using Common.Log;
//using Lykke.Service.EthereumSamurai.Core.Models;
//using Lykke.Service.EthereumSamurai.Core.Services;

//namespace Lykke.Job.EthereumSamurai.Jobs
//{
//    public class BlockIndexingJobFactory : IBlockIndexingJobFactory
//    {
//        private readonly IBlockService    _blockService;
//        private readonly IIndexingService _indexingService;
//        private readonly ILog             _logger;
//        private readonly IRpcBlockReader  _rpcBlockReader;

//        public BlockIndexingJobFactory(
//            IBlockService    blockService,
//            IIndexingService indexingService,
//            ILog             logger,
//            IRpcBlockReader  rpcBlockReader)
//        {
//            _blockService    = blockService;
//            _indexingService = indexingService;
//            _logger          = logger;
//            _rpcBlockReader  = rpcBlockReader;
//        }

//        public IJob GetJob(IIndexingSettings settings)
//        {
//            return new BlockIndexingJob(_blockService, _indexingService, settings, _logger, _rpcBlockReader);
//        }
//    }
//}