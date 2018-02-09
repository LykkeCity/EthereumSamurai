using System;
using System.Threading;
using Lykke.Logs;

namespace Lykke.Service.EthereumSamurai.Logger
{
    public abstract class LykkeLogEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LykkeLogEvent" /> class.
        /// </summary>
        protected LykkeLogEvent()
        {
            Timestamp = DateTime.UtcNow;
            Thread = Thread.CurrentThread;
        }

        /// <summary>
        ///     The duration of operation.
        /// </summary>
        public long? Duration { get; protected set; }

        /// <summary>
        ///     The type that generated this event.
        /// </summary>
        public Type LogClass { get; protected set; }

        /// <summary>
        ///     The <see cref="Logs.LogLevel" /> used to classify this event.
        /// </summary>
        public abstract LogLevel LogLevel { get; }

        /// <summary>
        ///     The source that generated this event.
        /// </summary>
        public string LogSource { get; protected set; }

        /// <summary>
        ///     The message associated with this event.
        /// </summary>
        public string Message { get; protected set; }

        /// <summary>
        ///     The method where this event occured.
        /// </summary>
        public string Process { get; protected set; }

        /// <summary>
        ///     The thread where this event occurred.
        /// </summary>
        public Thread Thread { get; }

        /// <summary>
        ///     The timestamp that this event occurred.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        ///     The message that triggered this event.
        /// </summary>
        public object Trigger { get; protected set; }
    }
}
