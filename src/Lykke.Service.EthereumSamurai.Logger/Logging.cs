using Akka.Actor;

namespace Lykke.Service.EthereumSamurai.Logger
{
    public static class Logging
    {
        /// <summary>
        ///     Creates a new logging adapter using the specified context's event stream.
        /// </summary>
        /// <param name="context">
        ///     The context used to configure the logging adapter.
        /// </param>
        /// <returns>
        ///     The newly created logging adapter.
        /// </returns>
        public static ILykkeLoggingAdapter GetLykkeLogger(this IActorContext context)
        {
            var logSource = context.Self.ToString();
            var logClass = context.Props.Type;

            return new LykkeBusLogging(context.System.EventStream, logSource, logClass);
        }
    }
}
