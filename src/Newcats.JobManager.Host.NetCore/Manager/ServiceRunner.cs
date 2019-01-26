using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Topshelf;

namespace Newcats.JobManager.Host.NetCore.Manager
{
    public class ServiceRunner : IHostedService, IDisposable
    {
        private readonly ILog _log;
        private ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;

        public ServiceRunner()
        {
            _log = LogManager.GetLogger(nameof(ServiceRunner));
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                _schedulerFactory = new StdSchedulerFactory();
                _scheduler = await _schedulerFactory.GetScheduler().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _log.Error($"Server initialization failed: {e.Message}", e);
            }
        }

        public bool Start(HostControl hostControl)
        {
            try
            {
                _scheduler.ListenerManager.AddJobListener(new JobListener(), GroupMatcher<JobKey>.AnyGroup());
                _scheduler.Start();
                QuartzManager.ManagerScheduler(_scheduler);
            }
            catch (Exception e)
            {
                _log.Error($"Scheduler started failed: {e.Message}", e);
            }
            _log.Info("Scheduler started successfully");
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            try
            {
                _scheduler.Shutdown(true);
            }
            catch (Exception e)
            {
                _log.Error($"Scheduler stop failed: {e.Message}", e);
            }
            _log.Info("Scheduler shutdown complete");
            return true;
        }

        /// <summary>
        /// TopShelf's method delegated to <see cref="Pause()"/>.
        /// </summary>
        public bool Pause(HostControl hostControl)
        {
            try
            {
                _scheduler.PauseAll();
                _log.Info("Scheduler Pause complete");
            }
            catch (Exception e)
            {
                _log.Info("Scheduler Pause failed", e);
            }
            return true;
        }

        /// <summary>
        /// TopShelf's method delegated to <see cref="Resume()"/>.
        /// </summary>
        public bool Continue(HostControl hostControl)
        {
            try
            {
                _scheduler.ResumeAll();
                _log.Info("Scheduler Continue complete");
            }
            catch (Exception e)
            {
                _log.Info("Scheduler Continue failed", e);
            }
            return true;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
