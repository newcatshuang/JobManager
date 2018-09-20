using System;
using System.Threading.Tasks;
using log4net;
using Quartz;

namespace Newcats.JobManager.Host.Manager
{
    [DisallowConcurrentExecution]
    public class SystemJob : IJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(SystemJob));
        public Task Execute(IJobExecutionContext context)
        {
            Version Ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            _logger.InfoFormat("ManagerJob Execute begin Ver." + Ver.ToString());
            try
            {
                new QuartzManager().JobScheduler(context.Scheduler);
                _logger.InfoFormat("ManagerJob Executing ...");
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
                _logger.InfoFormat("ManagerJob Execute end ");
            }
            //Console.WriteLine($"hello system job at {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}