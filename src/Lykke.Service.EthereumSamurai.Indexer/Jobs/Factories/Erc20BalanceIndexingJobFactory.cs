using Common.Log;
using Lykke.Service.EthereumSamurai.Core.Services;

namespace Lykke.Service.EthereumSamurai.Indexer.Jobs
{
    public class Erc20BalanceIndexingJobFactory : IErc20BalanceIndexingJobFactory
    {
        private readonly IErc20BalanceIndexingService _indexingService;
        private readonly ILog                         _logger;


        public Erc20BalanceIndexingJobFactory(
            IErc20BalanceIndexingService indexingService,
            ILog                         logger)
        {
            _indexingService = indexingService;
            _logger          = logger;
        }

        public IJob GetJob(ulong startFrom)
        {
            return new Erc20BalanceIndexingJob(_indexingService, _logger, startFrom);
        }
    }
}