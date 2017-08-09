namespace EthereumSamurai.Models.Query
{
    public class Erc20TransferHistoryQuery
    {
        public string ContractAddress { get; set; }

        public int? Count { get; set; }

        public ulong? FromBlockNumber { get; set; }

        public ulong? ToBlockNumber { get; set; }
        
        public int? Start { get; set; }

        public string TransactionHash { get; set; }

        public string TransfereeAddress { get; set; }

        public string TransferorAddress { get; set; }
    }
}