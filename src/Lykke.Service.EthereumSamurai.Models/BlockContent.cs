using System.Collections.Generic;
using Lykke.Service.EthereumSamurai.Models.Blockchain;

namespace Lykke.Service.EthereumSamurai.Models
{
    public class BlockContent
    {
        public IEnumerable<AddressHistoryModel> AddressHistory { get; set; }

        public BlockModel BlockModel { get; set; }

        public List<DeployedContractModel> DeployedContracts { get; set; }

        public List<InternalMessageModel> InternalMessages { get; set; }

        public List<TransactionModel> Transactions { get; set; }

        public List<Erc20TransferHistoryModel> Transfers { get; set; }
    }
}