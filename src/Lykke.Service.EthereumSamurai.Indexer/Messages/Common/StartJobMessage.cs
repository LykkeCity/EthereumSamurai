using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Messages
{
    public static partial class Common
    {
        public static StartJobMessage CreateStartJobMessage()
        {
            return new StartJobMessage();
        }

        public class StartJobMessage
        {
            public StartJobMessage()
            {
            }
        }
    }
}
