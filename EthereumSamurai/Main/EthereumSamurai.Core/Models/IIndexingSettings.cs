using System.Numerics;

namespace EthereumSamurai.Core.Models
{
    public interface IIndexingSettings
    {
        BigInteger From { get; set; }

        string IndexerId { get; set; }

        BigInteger? To { get; set; }
    }
}