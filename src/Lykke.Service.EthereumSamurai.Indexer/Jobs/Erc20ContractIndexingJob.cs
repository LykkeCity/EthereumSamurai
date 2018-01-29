using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Indexer.Utils;

namespace Lykke.Service.EthereumSamurai.Indexer.Jobs
{
    public class Erc20ContractIndexingJob : IJob
    {
        private readonly IErc20ContractIndexingService _indexingService;
        private readonly ILog                          _logger;


        public Erc20ContractIndexingJob(
            IErc20ContractIndexingService indexingService,
            ILog logger)
        {
            _indexingService = indexingService;
            _logger          = logger;
        }


        public string Id
            => nameof(Erc20ContractIndexingJob);

        public int Version
            => 1;


        public Task RunAsync()
        {
            return RunAsync(CancellationToken.None);
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(async () =>
            {

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var contract = await _indexingService.GetNextContractToIndexAsync();

                        await RetryPolicy.ExecuteAsync(async () =>
                        {

                            await _indexingService.IndexContractAsync(contract);

                        }, 5, 100);

                        await _logger.WriteInfoAsync
                        (
                            nameof(Erc20ContractIndexingJob),
                            nameof(RunAsync),
                            "Contract indexed",
                            $"Indexed contract as address {contract.Address}.",
                            DateTime.UtcNow
                        );
                    }
                }
                catch (Exception e)
                {
                    await _logger.WriteErrorAsync
                    (
                        nameof(Erc20ContractIndexingJob),
                        nameof(RunAsync),
                        "Indexing failed",
                        e,
                        DateTime.UtcNow
                    );

                    throw;
                }


            }, cancellationToken).Unwrap();
        }
    }
}