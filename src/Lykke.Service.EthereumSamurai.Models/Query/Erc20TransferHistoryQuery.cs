using System.Collections.Generic;

namespace Lykke.Service.EthereumSamurai.Models.Query
{
    public class Erc20TransferHistoryQuery
    {
        public string AssetHolder { get; set; }

        public ulong? BlockNumber { get; set; }

        public IEnumerable<string> Contracts { get; set; }

        public int? Count { get; set; }

        public int? Start { get; set; }
        public string TransactionHash { get; set; }
    }
}