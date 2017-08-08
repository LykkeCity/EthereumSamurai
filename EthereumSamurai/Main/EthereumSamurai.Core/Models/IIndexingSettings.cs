using System.Numerics;

namespace EthereumSamurai.Core.Models
{
    public interface IIndexingSettings
    {
        string IndexerId { get; set; }

        BigInteger From { get; set; }

        BigInteger? To { get; set; }
    }
}