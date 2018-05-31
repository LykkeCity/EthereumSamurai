using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.EthereumSamurai.Logger;

namespace Lykke.Job.EthereumSamurai.Extensions
{
    public static class ContextExtensions
    {
        public static LykkeLogBuilder<T> GetLogger<T>(this IUntypedActorContext context, T message, string process = "")
        {
            return new LykkeLogBuilder<T>(context, process, message);
        }
    }
}
