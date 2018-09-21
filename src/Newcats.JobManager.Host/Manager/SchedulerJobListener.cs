using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newcats.JobManager.Host.Domain.Service;
using Quartz;

namespace Newcats.JobManager.Host.Manager
{
    class SchedulerJobListener : IJobListener
    {
        public string Name => "SchedulerJobListener";

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            long jobId = Convert.ToInt64(context.JobDetail.Key.Name);
            DateTime nextFireTime = TimeZoneInfo.ConvertTimeFromUtc(context.NextFireTimeUtc.Value.DateTime, TimeZoneInfo.Local);
            DateTime lastFireTime = TimeZoneInfo.ConvertTimeFromUtc(context.FireTimeUtc.DateTime, TimeZoneInfo.Local);
            double TotalSeconds = context.JobRunTime.TotalSeconds;
            string logContent = string.Empty;
            if (jobException != null)
                logContent = $"Job执行异常，异常信息:{jobException.Message}";
            JobService.UpdateJobStatus(jobId, lastFireTime, nextFireTime, TotalSeconds, logContent);
            return Task.CompletedTask;
        }
    }
}
