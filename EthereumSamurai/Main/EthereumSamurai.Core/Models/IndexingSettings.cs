using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace EthereumSamurai.Core.Models
{
    public interface IIndexingSettings
    {
        BigInteger From { get; set; }
        BigInteger? To { get; set; }
    }

    public class IndexingSettings : IIndexingSettings
    {
        public BigInteger From { get; set; }
        public BigInteger? To { get; set; }
    }
}
