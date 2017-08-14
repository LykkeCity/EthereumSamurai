namespace EthereumSamurai.Core.Settings
{
    public class IndexerInstanceSettings : IIndexerInstanceSettings
    {
        public ulong BalancesStartBlock { get; set; }

        public string IndexerId { get; set; }

        public ulong StartBlock { get; set; }

        public ulong? StopBlock { get; set; }

        public int ThreadAmount { get; set; }
    }
}