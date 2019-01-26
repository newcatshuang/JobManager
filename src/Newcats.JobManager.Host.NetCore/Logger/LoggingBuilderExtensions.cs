using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Newcats.JobManager.Host.NetCore.Logger
{
    /// <summary>
    ///     Extensions to the <see cref="ILoggingBuilder" />
    /// </summary>
    public static class LoggingBuilderExtensions
    {
        /// <summary>
        ///     Adds the log4net logging provider.
        /// </summary>
        /// <param name="builder">The logging builder instance.</param>
        /// <param name="configure">
        ///     An optional log4net configuration action invoked before the provider is added to the
        ///     <see cref="ILoggingBuilder" />.
        /// </param>
        /// <returns>The <see ref="ILoggingBuilder" /> passed as parameter with the new provider registered.</returns>
        public static ILoggingBuilder AddLog4Net(this ILoggingBuilder builder, Action configure = null)
        {
            configure?.Invoke();
            builder.Services.AddSingleton<ILoggerProvider>(new Log4NetProvider());

            return builder;
        }
    }
}
