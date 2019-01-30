using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace Newcats.JobManager.Host.NetCore.Manager
{
    public class ServiceRunner : IHostedService
    {
        private ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;

        public ServiceRunner()
        {
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
                //_log.Error($"Server initialization failed: {e.Message}", e);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            try
            {
                _scheduler.ListenerManager.AddJobListener(new JobListener(), GroupMatcher<JobKey>.AnyGroup());
                await _scheduler.Start();
                await QuartzManager.ManagerScheduler(_scheduler);
            }
            catch (Exception e)
            {
                //_log.Error($"Scheduler started failed: {e.Message}", e);
            }
            //_log.Info("Scheduler started successfully");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            try
            {
                await _scheduler.Shutdown(true);
            }
            catch (Exception e)
            {
                //_log.Error($"Scheduler stop failed: {e.Message}", e);
            }
            //_log.Info("Scheduler shutdown complete");
        }
    }
}