using System.Threading.Tasks;
using Quartz;

namespace Newcats.System.Job.DeleteSystemLog
{
    /// <summary>
    /// 删除一个月前的JobLevel.System级的JobLog
    /// </summary>
    public class DeleteSystemLogJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            new DeleteService().DeleteSystemLog();
            return Task.CompletedTask;
        }
    }
}