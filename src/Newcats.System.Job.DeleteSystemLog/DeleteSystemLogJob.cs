using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newcats.JobManager.Common;
using Quartz;

namespace Newcats.System.Job.DeleteSystemLog
{
    public class DeleteSystemLogJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}