using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Messages
{
    public static partial class Common
    {
        public static DoIterationMessage CreateDoIterationMessage()
        {
            return new DoIterationMessage();
        }

        public class DoIterationMessage
        {
            public DoIterationMessage()
            {
            }
        }
    }
}
