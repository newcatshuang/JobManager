using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace Newcats.JobManager.Host.NetCore.Manager
{
    public class ServiceRunner : IHostedService
    {
        private readonly ILogger _log;
        private ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;
        private readonly IQuartzManager _quartzManager;
        private readonly IJobListener _jobListener;

        public ServiceRunner(ILogger<ServiceRunner> log, IQuartzManager quartzManager, IJobListener jobListener)
        {
            _log = log;
            _quartzManager = quartzManager;
            _jobListener = jobListener;
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
                _log.LogError($"Server initialization failed: {e.Message}", e);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return; //Task.FromCanceled(cancellationToken);

            try
            {
                _scheduler.ListenerManager.AddJobListener(_jobListener, GroupMatcher<JobKey>.AnyGroup());
                await _scheduler.Start();
                await _quartzManager.ManagerScheduler(_scheduler);
            }
            catch (Exception e)
            {
                _log.LogError($"Scheduler started failed: {e.Message}", e);
            }
            _log.LogError("Scheduler started successfully");
            //return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;//Task.FromCanceled(cancellationToken);

            try
            {
                await _scheduler.Shutdown(true);
            }
            catch (Exception e)
            {
                _log.LogError($"Scheduler stop failed: {e.Message}", e);
            }
            _log.LogError("Scheduler shutdown complete");
            // return Task.CompletedTask;
        }
    }
}