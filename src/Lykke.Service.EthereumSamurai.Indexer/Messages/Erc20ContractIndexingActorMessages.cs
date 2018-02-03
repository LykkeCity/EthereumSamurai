using Lykke.Service.EthereumSamurai.Models.Blockchain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Messages
{
    public partial class Erc20ContractIndexingActor
    {
        public static Erc20ContractDeployedMessage CreateErc20ContractDeployedMessage(DeployedContractModel deployedContractModel)
        {
            return new Erc20ContractDeployedMessage(deployedContractModel);
        }

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
}
