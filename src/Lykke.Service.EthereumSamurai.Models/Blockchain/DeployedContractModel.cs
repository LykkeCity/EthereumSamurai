namespace Lykke.Service.EthereumSamurai.Models.Blockchain
{
    public class DeployedContractModel
    {
        public string Address { get; set; }
        
        public string BlockHash { get; set; }

        public string BlockNumber { get; set; }

        public string BlockTimestamp { get; set; }

        public string DeployerAddress { get; set; }

        public string TransactionHash { get; set; }
    }
}