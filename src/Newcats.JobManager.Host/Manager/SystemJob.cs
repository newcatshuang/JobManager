using System;
using System.Threading.Tasks;
using log4net;
using Quartz;

namespace Newcats.JobManager.Host.Manager
{
    /// <summary>
    /// 系统Job，负责管理所有其他Job，不能停止/禁用/删除
    /// </summary>
    [DisallowConcurrentExecution]
    public class SystemJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(SystemJob));
        public Task Execute(IJobExecutionContext context)
        {
            Version Ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            _logger.InfoFormat("SystemJob Execute begin Ver." + Ver.ToString());
            try
            {
                QuartzManager.ManagerScheduler(context.Scheduler);
                _logger.InfoFormat("SystemJob Executing ...");
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
                _logger.InfoFormat("SystemJob Execute end. ");
            }
            return Task.CompletedTask;
        }
    }
}