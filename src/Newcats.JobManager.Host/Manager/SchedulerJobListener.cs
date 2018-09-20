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
            DateTime NextFireTimeUtc = TimeZoneInfo.ConvertTimeFromUtc(context.NextFireTimeUtc.Value.DateTime, TimeZoneInfo.Local);
            DateTime FireTimeUtc = TimeZoneInfo.ConvertTimeFromUtc(context.FireTimeUtc.DateTime, TimeZoneInfo.Local);

            double TotalSeconds = context.JobRunTime.TotalSeconds;
            string JobName = string.Empty;
            string LogContent = string.Empty;
            if (context.MergedJobDataMap != null)
            {
                JobName = context.MergedJobDataMap.GetString("JobName");
                StringBuilder log = new StringBuilder();
                int i = 0;
                foreach (var item in context.MergedJobDataMap)
                {
                    string key = item.Key;
                    if (key.StartsWith("extend_"))
                    {
                        if (i > 0)
                        {
                            log.Append(",");
                        }
                        log.AppendFormat("{0}:{1}", item.Key, item.Value);
                        i++;
                    }
                }
                if (i > 0)
                {
                    LogContent = string.Concat("[", log.ToString(), "]");
                }
            }
            if (jobException != null)
            {
                LogContent = LogContent + " EX:" + jobException.ToString();
            }

            //new BackgroundJobService().UpdateBackgroundJobStatus(BackgroundJobId, JobName, FireTimeUtc, NextFireTimeUtc, TotalSeconds, LogContent);

            JobService.UpdateJobStatus(jobId, FireTimeUtc, NextFireTimeUtc, TotalSeconds, LogContent);
            return Task.CompletedTask;
        }
    }
}
