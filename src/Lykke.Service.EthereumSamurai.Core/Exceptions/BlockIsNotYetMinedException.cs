using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Service.EthereumSamurai.Core.Exceptions
{
    public class BlockIsNotYetMinedException : Exception
    {
        public BlockIsNotYetMinedException(BigInteger blockNumber) : base($"Block[{blockNumber}] is not yet mined")
        { }
    }
}
