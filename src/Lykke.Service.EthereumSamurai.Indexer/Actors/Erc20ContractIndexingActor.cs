using Akka.Actor;
using Lykke.Job.EthereumSamurai.Extensions;
using Lykke.Job.EthereumSamurai.Messages;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Core.Utils;

namespace Lykke.Job.EthereumSamurai.Actors
{
    public class Erc20ContractIndexingActor : ReceiveActor
    {
        private readonly IErc20ContractIndexingService _indexingService;

        public Erc20ContractIndexingActor(
            IErc20ContractIndexingService indexingService)
        {
            _indexingService = indexingService;

            ReceiveAsync<Erc20ContractDeployedMessage>(async (message) =>
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
    }
}
