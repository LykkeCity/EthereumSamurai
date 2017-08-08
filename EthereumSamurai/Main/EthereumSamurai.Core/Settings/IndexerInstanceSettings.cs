namespace EthereumSamurai.Core.Settings
{
    public class IndexerInstanceSettings : IIndexerInstanceSettings
    {
        public string IndexerId { get; set; }

        public ulong StartBlock { get; set; }

        public ulong? StopBlock { get; set; }

        public int ThreadAmount { get; set; }
    }
}