namespace EthereumSamurai.Core.Settings
{
    public interface IIndexerInstanceSettings
    {
        string IndexerId { get; set; }

        ulong StartBlock { get; set; }

        ulong? StopBlock { get; set; }

        int ThreadAmount { get; set; }
    }
}