using System;
using Lykke.Logs;

namespace Lykke.Service.EthereumSamurai.Logger
{
    /// <summary>
    ///     This class represents an Info log event.
    /// </summary>
    public sealed class LykkeInfo : LykkeLogEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LykkeInfo" /> class.
        /// </summary>
        /// <param name="logSource">
        ///     The source that generated event.
        /// </param>
        /// <param name="logClass">
        ///     The type that generated event.
        /// </param>
        /// <param name="message">
        ///     The message that is being logged.
        /// </param>
        /// <param name="duration">
        ///     The duration of operation.
        /// </param>
        /// <param name="process">
        ///     The method where event occured.
        /// </param>
        /// <param name="trigger">
        ///     The message that triggered this event.
        /// </param>
        /// <inheritdoc />
        public LykkeInfo(string logSource, Type logClass, string message, long? duration, string process, object trigger)
        {
            Duration = duration;
            LogSource = logSource;
            LogClass = logClass;
            Message = message;
            Process = process;
            Trigger = trigger;
        }

        /// <inheritdoc />
        public override LogLevel LogLevel { get; }
            = LogLevel.Info;
    }
}
