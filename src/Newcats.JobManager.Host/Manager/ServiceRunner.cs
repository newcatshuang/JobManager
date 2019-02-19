using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace Newcats.JobManager.Host.Manager
{
    /// <summary>
    /// 服务执行者
    /// </summary>
    public class ServiceRunner : IHostedService
    {
        private ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ServiceRunner()
        {
            Initialize();
        }

        /// <summary>
        /// 初始化
        /// </summary>
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

        /// <summary>
        /// 服务启动
        /// </summary>
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

        /// <summary>
        /// 服务停止
        /// </summary>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            try
            {
                QuartzManager.SchedulerStopping();
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