using System.Numerics;

namespace Lykke.Service.EthereumSamurai.Core.Models
{
    public interface IIndexingSettings
    {
        BigInteger From { get; set; }

        string IndexerId { get; set; }

        BigInteger? To { get; set; }
    }
}