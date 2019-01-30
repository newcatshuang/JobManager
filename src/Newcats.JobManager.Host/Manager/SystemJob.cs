using System;
using System.Threading.Tasks;
using Quartz;

namespace Newcats.JobManager.Host.Manager
{
    /// <summary>
    /// 系统Job，负责管理所有其他Job，不能停止/禁用/删除
    /// </summary>
    [DisallowConcurrentExecution]
    public class SystemJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Version Ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            //_log.Info("SystemJob Execute begin Ver." + Ver.ToString());
            try
            {
                await QuartzManager.ManagerScheduler(context.Scheduler);
                //_log.Info("SystemJob Executing ...");
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
                //_log.Info("SystemJob Execute end. ");
            }
        }
    }
}