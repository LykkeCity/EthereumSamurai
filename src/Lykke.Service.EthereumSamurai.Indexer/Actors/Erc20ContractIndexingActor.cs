using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.EthereumSamurai.Core.Services;
using Akka.Actor;
using Messages = Lykke.Job.EthereumSamurai.Messages;
using Lykke.Service.EthereumSamurai.Core.Utils;
using Lykke.Job.EthereumSamurai.Extensions;

namespace Lykke.Job.EthereumSamurai.Jobs
{
    public partial class Erc20ContractIndexingActor : ReceiveActor
    {
        private readonly IErc20ContractIndexingService _indexingService;

        public Erc20ContractIndexingActor(
            IErc20ContractIndexingService indexingService)
        {
            _indexingService = indexingService;

            ReceiveAsync<Messages.Erc20ContractIndexingActor.Erc20ContractDeployedMessage>(async (message) =>
            {
                using (var logger = Context.GetLogger(message))
                {
                    var contractModel = message.DeployedContractModel;

                    await RetryPolicy.ExecuteAsync(async () =>
                    {
                        await _indexingService.IndexContractAsync(contractModel);

                    }, 5, 100);

                    logger.Info($"Indexed contract as address {contractModel.Address}.");
                }
            });
        }

        public string Id
            => nameof(Erc20ContractIndexingJob);

        public int Version
            => 1;
    }
}