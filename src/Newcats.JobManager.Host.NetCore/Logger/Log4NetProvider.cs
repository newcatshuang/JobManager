using System;
using System.Collections.Concurrent;
using System.Reflection;
using log4net;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.Logging;

namespace Newcats.JobManager.Host.NetCore.Logger
{
    /// <summary>
    ///     The log4net provider class.
    /// </summary>
    public class Log4NetProvider : ILoggerProvider
    {
        private readonly ILoggerRepository loggerRepository;
        private readonly ConcurrentDictionary<string, Log4NetLogger> loggers = new ConcurrentDictionary<string, Log4NetLogger>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="Log4NetProvider" /> class.
        /// </summary>
        public Log4NetProvider()
        {
            loggerRepository = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(Hierarchy));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, _ => new Log4NetLogger(loggerRepository.Name, categoryName));
        }

        /// <summary>
        ///     Dispose pattern.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}
