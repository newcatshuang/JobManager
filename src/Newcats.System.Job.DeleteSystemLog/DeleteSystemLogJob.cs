using System.Threading.Tasks;
using Quartz;

namespace Newcats.System.Job.DeleteSystemLog
{
    public class DeleteSystemLogJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            new DeleteService().DeleteSystemLog();
            return Task.CompletedTask;
        }
    }
}