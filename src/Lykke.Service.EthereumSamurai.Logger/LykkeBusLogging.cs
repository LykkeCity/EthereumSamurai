using System;
using Akka.Event;

namespace Lykke.Service.EthereumSamurai.Logger
{
    public class LykkeBusLogging : ILykkeLoggingAdapter
    {
        private readonly LoggingBus _bus;
        private readonly Type _logClass;
        private readonly string _logSource;


        public LykkeBusLogging(
            LoggingBus bus,
            string logSource,
            Type logClass)
        {
            _bus = bus;
            _logSource = logSource;
            _logClass = logClass;
        }


        public void Info(string message, long? duration = null, string process = "", object trigger = null)
        {
            _bus.Publish(new LykkeInfo
            (
                _logSource,
                _logClass,
                message,
                duration,
                process,
                trigger
            ));
        }

        public void Warning(string message, long? duration = null, string process = "", object trigger = null,
            Exception cause = null)
        {
            _bus.Publish(new LykkeWarning
            (
                _logSource,
                _logClass,
                message,
                duration,
                process,
                trigger,
                cause
            ));
        }

        public void Error(string message, long? duration = null, string process = "", object trigger = null,
            Exception cause = null)
        {
            _bus.Publish(new LykkeError
            (
                _logSource,
                _logClass,
                message,
                duration,
                process,
                trigger,
                cause
            ));
        }

        public void FatalError(string message, long? duration = null, string process = "", object trigger = null,
            Exception cause = null)
        {
            _bus.Publish(new LykkeFatalError
            (
                _logSource,
                _logClass,
                message,
                duration,
                process,
                trigger,
                cause
            ));
        }

        public void Monitoring(string message, long? duration = null, string process = "", object trigger = null)
        {
            _bus.Publish(new LykkeMonitoring
            (
                _logSource,
                _logClass,
                message,
                duration,
                process,
                trigger
            ));
        }
    }
}
