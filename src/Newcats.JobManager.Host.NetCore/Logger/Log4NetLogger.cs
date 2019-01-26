using System;
using log4net;
using Microsoft.Extensions.Logging;

namespace Newcats.JobManager.Host.NetCore.Logger
{
    /// <summary>
    /// The log4net logger class.
    /// </summary>
    public class Log4NetLogger : ILogger
    {
        private readonly ILog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLogger"/> class.
        /// </summary>
        /// <param name="loggerRepository">The repository name.</param>
        /// <param name="name">The logger's name.</param>
        public Log4NetLogger(string loggerRepository, string name)
        {
            log = LogManager.GetLogger(loggerRepository, name);
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return log.IsFatalEnabled;
                case LogLevel.Debug:
                case LogLevel.Trace:
                    return log.IsDebugEnabled;
                case LogLevel.Error:
                    return log.IsErrorEnabled;
                case LogLevel.Information:
                    return log.IsInfoEnabled;
                case LogLevel.Warning:
                    return log.IsWarnEnabled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        /// <inheritdoc />
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);
            if (!string.IsNullOrEmpty(message) || exception != null)
                switch (logLevel)
                {
                    case LogLevel.Critical:
                        log.Fatal(message, exception);
                        break;

                    case LogLevel.Debug:
                        log.Debug(message, exception);
                        break;

                    case LogLevel.Error:
                        log.Error(message, exception);
                        break;

                    case LogLevel.Information:
                        log.Info(message, exception);
                        break;

                    case LogLevel.Warning:
                        log.Warn(message, exception);
                        break;

                    case LogLevel.Trace:
                        log.Debug(message, exception);
                        break;

                    default:
                        log.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
                        log.Info(message, exception);
                        break;
                }
        }
    }
}