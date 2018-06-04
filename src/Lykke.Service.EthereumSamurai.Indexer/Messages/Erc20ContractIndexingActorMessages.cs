using System.ComponentModel;
using Lykke.Service.EthereumSamurai.Models.Blockchain;

namespace Lykke.Job.EthereumSamurai.Messages
{
    [ImmutableObject(true)]
    public class Erc20ContractDeployedMessage
    {
        public Erc20ContractDeployedMessage(DeployedContractModel deployedContractModel)
        {
            DeployedContractModel = deployedContractModel;
        }

        public DeployedContractModel DeployedContractModel { get; private set; }
    }
}
