using System;
using System.Threading.Tasks;
using Newcats.JobManager.Common.NetCore.DataAccess;
using Newcats.JobManager.Common.NetCore.Entity;
using Quartz;

namespace QuartzJobInGenericHostWithTopshelf
{
    public class WriteLogJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Repository<JobLogEntity, long> repository = new Repository<JobLogEntity, long>();
            repository.Insert(new JobLogEntity()
            {
                JobId = 4,
                FireTime = DateTime.Now,
                FireDuration = 0,
                FireState = FireState.Success,
                Content = "测试QuartzJobInGenericHostWithTopshelf",
                CreateTime = DateTime.Now
            });

            return Task.CompletedTask;
        }
    }
}