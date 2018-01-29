namespace Lykke.Service.EthereumSamurai.Core.Settings
{
    public interface IIndexerInstanceSettings
    {
        ulong BalancesStartBlock { get; set; }

        bool IndexBalances { get; set; }

        bool IndexBlocks { get; set; }

        string IndexerId { get; set; }

        ulong StartBlock { get; set; }

        ulong? StopBlock { get; set; }

        int ThreadAmount { get; set; }

        bool SendEventsToRabbit { get; set; }

        bool IndexContracts { get; set; }

        int ContractsIndexerThreadAmount { get; set; }
    }
}