namespace EthereumSamurai.Core.Settings
{
    public interface IIndexerInstanceSettings
    {
        ulong BalancesStartBlock { get; set; }

        string IndexerId { get; set; }

        ulong StartBlock { get; set; }

        ulong? StopBlock { get; set; }

        int ThreadAmount { get; set; }
    }
}