using System;
using Lykke.Logs;

namespace Lykke.Service.EthereumSamurai.Logger
{
    /// <summary>
    ///     This class represents an Monitoring log event.
    /// </summary>
    public sealed class LykkeMonitoring : LykkeLogEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LykkeMonitoring" /> class.
        /// </summary>
        /// <param name="logSource">
        ///     The source that generated the event.
        /// </param>
        /// <param name="logClass">
        ///     The type that generated the event.
        /// </param>
        /// <param name="message">
        ///     The message that is being logged.
        /// </param>
        /// <param name="duration">
        ///     The duration of operation.
        /// </param>
        /// <param name="process">
        ///     The method where the event occured.
        /// </param>
        /// <param name="trigger">
        ///     The message that triggered the event.
        /// </param>
        /// <inheritdoc />
        public LykkeMonitoring(string logSource, Type logClass, string message, long? duration, string process,
            object trigger)
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
            = LogLevel.Monitoring;
    }
}
