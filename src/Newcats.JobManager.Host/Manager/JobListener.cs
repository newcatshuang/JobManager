using System;
using System.Threading;
using System.Threading.Tasks;
using Newcats.JobManager.Common.Entity;
using Newcats.JobManager.Host.Service;
using Quartz;

namespace Newcats.JobManager.Host.Manager
{
    /// <summary>
    /// 监听器,写Job运行日志
    /// </summary>
    public class JobListener : IJobListener
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
            try
            {
                DateTime nextFireTime = TimeZoneInfo.ConvertTimeFromUtc(context.NextFireTimeUtc.HasValue ? context.NextFireTimeUtc.Value.UtcDateTime : default(DateTime), TimeZoneInfo.Local);
                JobLogEntity logEntity = new JobLogEntity
                {
                    JobId = Convert.ToInt32(context.JobDetail.Key.Name),
                    FireTime = TimeZoneInfo.ConvertTimeFromUtc(context.FireTimeUtc.UtcDateTime, TimeZoneInfo.Local),
                    FireDuration = context.JobRunTime.TotalSeconds
                };
                if (jobException != null)
                {
                    logEntity.FireState = FireState.Error;
                    logEntity.Content = $"Job执行异常，异常信息:{jobException.Message}";
                }
                else
                {
                    logEntity.FireState = FireState.Success;
                    logEntity.Content = "success";
                }
                logEntity.CreateTime = DateTime.Now;
                new JobService().UpdateJobFireResult(nextFireTime, logEntity);
            }
            catch { }
            return Task.CompletedTask;
        }
    }
}
