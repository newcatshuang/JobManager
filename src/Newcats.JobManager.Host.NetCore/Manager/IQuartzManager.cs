using System.Threading.Tasks;
using Quartz;

namespace Newcats.JobManager.Host.NetCore.Manager
{
    public interface IQuartzManager
    {
        Task ManagerScheduler(IScheduler scheduler);
    }
}