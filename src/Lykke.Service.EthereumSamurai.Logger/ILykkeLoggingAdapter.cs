using System;
using Lykke.Logs;

namespace Lykke.Service.EthereumSamurai.Logger
{
    public interface ILykkeLoggingAdapter
    {
        /// <summary>
        ///     Logs a <see cref="LogLevel.Error" /> message.
        /// </summary>
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
        ///     The message that triggered the log event.
        /// </param>
        /// <param name="cause">
        ///     The exception that caused the log event.
        /// </param>
        void Error(string message, long? duration = null, string process = "", object trigger = null,
            Exception cause = null);

        /// <summary>
        ///     Logs a <see cref="LogLevel.FatalError" /> message.
        /// </summary>
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
        ///     The message that triggered the log event.
        /// </param>
        /// <param name="cause">
        ///     The exception that caused the log event.
        /// </param>
        void FatalError(string message, long? duration = null, string process = "", object trigger = null,
            Exception cause = null);

        /// <summary>
        ///     Logs a <see cref="LogLevel.Info" /> message.
        /// </summary>
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
        void Info(string message, long? duration = null, string process = "", object trigger = null);

        /// <summary>
        ///     Logs a <see cref="LogLevel.Monitoring" /> message.
        /// </summary>
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
        void Monitoring(string message, long? duration = null, string process = "", object trigger = null);

        /// <summary>
        ///     Logs a <see cref="LogLevel.Warning" /> message.
        /// </summary>
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
        ///     The message that triggered the log event.
        /// </param>
        /// <param name="cause">
        ///     The exception that caused the log event.
        /// </param>
        void Warning(string message, long? duration = null, string process = "", object trigger = null,
            Exception cause = null);
    }
}
