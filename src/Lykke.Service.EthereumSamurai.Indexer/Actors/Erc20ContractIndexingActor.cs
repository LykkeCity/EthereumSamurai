using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.EthereumSamurai.Core.Services;
using Akka.Actor;
using Messages = Lykke.Job.EthereumSamurai.Messages;
using Lykke.Service.EthereumSamurai.Core.Utils;

namespace Lykke.Job.EthereumSamurai.Jobs
{
    public partial class Erc20ContractIndexingActor : ReceiveActor
    {
        private readonly IErc20ContractIndexingService _indexingService;
        private readonly ILog _logger;

        public Erc20ContractIndexingActor(
            IErc20ContractIndexingService indexingService,
            ILog logger)
        {
            _indexingService = indexingService;
            _logger = logger;

            ReceiveAsync<Messages.Erc20ContractIndexingActor.Erc20ContractDeployedMessage>(async (message) =>
            {
                var contractModel = message.DeployedContractModel;

                await RetryPolicy.ExecuteAsync(async () =>
                {
                    await _indexingService.IndexContractAsync(contractModel);

                }, 5, 100);

                await _logger.WriteInfoAsync
                (
                    nameof(Erc20ContractIndexingJob),
                    nameof(ReceiveAsync),
                    "Contract indexed",
                    $"Indexed contract as address {contractModel.Address}.",
                    DateTime.UtcNow
                );
            });
        }

        public string Id
            => nameof(Erc20ContractIndexingJob);

        public int Version
            => 1;
    }
}