namespace Lykke.Service.EthereumSamurai.Core.Settings
{
    public class IndexerInstanceSettings : IIndexerInstanceSettings
    {
        public ulong BalancesStartBlock { get; set; }

        public bool IndexBalances { get; set; }

        public bool IndexBlocks { get; set; }

        public bool IndexContracts { get; set; }

        public string IndexerId { get; set; }

        public ulong StartBlock { get; set; }

        public ulong? StopBlock { get; set; }

        public int ThreadAmount { get; set; }

        public bool SendEventsToRabbit { get; set; }

        public int ContractsIndexerThreadAmount { get; set; }
    }
}