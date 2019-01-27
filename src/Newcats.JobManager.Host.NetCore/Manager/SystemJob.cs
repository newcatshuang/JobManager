using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Newcats.JobManager.Host.NetCore.Manager
{
    /// <summary>
    /// 系统Job，负责管理所有其他Job，不能停止/禁用/删除
    /// </summary>
    [DisallowConcurrentExecution]
    public class SystemJob : IJob
    {
        //private readonly ILogger _logger;
        //private readonly IQuartzManager _quartzManager;

        //public SystemJob(ILogger<SystemJob> logger, IQuartzManager quartzManager)
        //{
        //    _logger = logger;
        //    _quartzManager = quartzManager;
        //}

        public async Task Execute(IJobExecutionContext context)
        {
            Version Ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            //_logger.LogInformation("SystemJob Execute begin Ver." + Ver.ToString());
            try
            {
                await QuartzManager.ManagerScheduler(context.Scheduler);
                //_logger.LogInformation("SystemJob Executing ...");
            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex)
                {
                    RefireImmediately = true
                };
            }
            finally
            {
                //_logger.LogInformation("SystemJob Execute end. ");
            }
        }
    }
}