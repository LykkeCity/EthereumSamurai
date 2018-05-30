using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Service.EthereumSamurai.Core.Exceptions
{
    public class OperationTimedOutException : Exception
    {
        public OperationTimedOutException(string message) : base(message)
        { }
    }
}
