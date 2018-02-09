using Akka.Actor;
using Lykke.Logs;
using Lykke.Service.EthereumSamurai.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Lykke.Job.EthereumSamurai.Extensions
{
    public class LykkeLogBuilder<T> : IDisposable
    {
        private readonly IUntypedActorContext _context;
        private readonly string _process;
        private readonly Stopwatch _stopwatch;
        private readonly T _trigger;


        private string _customMessage;
        private Exception _exception;
        private LogLevel _logLevel;
        private bool _supressed;

        public LykkeLogBuilder(IUntypedActorContext context, string process, T trigger)
        {
            _context = context;
            _logLevel = LogLevel.Info;
            _process = string.IsNullOrEmpty(process) ? typeof(T).Name : process;
            _stopwatch = new Stopwatch();
            _trigger = trigger;

            _stopwatch.Start();
        }

        public void Error(Exception cause, string message = "")
        {
            _customMessage = message;
            _exception = cause;
            _logLevel = LogLevel.Error;
        }

        public void Info(string message = "")
        {
            _customMessage = message;
            _logLevel = LogLevel.Info;
        }

        public void SetMessage(string message)
        {
            _customMessage = message;
        }

        public void Suppress()
        {
            _supressed = true;
        }

        public void Warning(Exception cause = null, string message = "")
        {
            _exception = cause;
            _logLevel = LogLevel.Warning;
            _customMessage = message;
        }

        private string GetMessage()
        {
            if (!string.IsNullOrEmpty(_customMessage))
            {
                return _customMessage;
            }

            switch (_logLevel)
            {
                case LogLevel.Info:
                    return "Operation completed";
                case LogLevel.Warning:
                    return "Operation completed with a warning";
                case LogLevel.Error:
                    return "Operation failed";
                case LogLevel.FatalError:
                    return "Operation failed with a disaster";
                case LogLevel.Monitoring:
                    return "Operation completed";
                default:
                    return string.Empty;
            }
        }

        public void Dispose()
        {
            _stopwatch.Stop();

            if (!_supressed)
            {
                var duration = _stopwatch.ElapsedMilliseconds;
                var lykkeLogger = _context.GetLykkeLogger();
                var message = GetMessage();

                switch (_logLevel)
                {
                    case LogLevel.Info:
                        lykkeLogger.Info(message, duration, _process, _trigger);
                        break;
                    case LogLevel.Warning:
                        lykkeLogger.Warning(message, duration, _process, _trigger, _exception);
                        break;
                    case LogLevel.Error:
                        lykkeLogger.Error(message, duration, _process, _trigger, _exception);
                        break;
                    case LogLevel.FatalError:
                        lykkeLogger.FatalError(message, duration, _process, _trigger, _exception);
                        break;
                    case LogLevel.Monitoring:
                        lykkeLogger.Monitoring(message, duration, _process, _trigger);
                        break;
                }
            }
        }
    }
}
