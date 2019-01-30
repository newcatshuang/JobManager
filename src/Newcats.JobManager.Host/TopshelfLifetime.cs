using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Newcats.JobManager.Host
{
    public class TopshelfLifetime : IHostLifetime
    {
        private IApplicationLifetime ApplicationLifetime { get; }

        public TopshelfLifetime(IApplicationLifetime applicationLifetime, IServiceProvider serviceProvider)
        {
            ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}