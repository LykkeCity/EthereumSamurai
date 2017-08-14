using System.Numerics;

namespace EthereumSamurai.Core.Models
{
    public class IndexingSettings : IIndexingSettings
    {
        public BigInteger From { get; set; }

        public string IndexerId { get; set; }

        public BigInteger? To { get; set; }
    }
}