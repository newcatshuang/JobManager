using System.Collections.Generic;
using System.Threading.Tasks;
using Newcats.JobManager.Api.Domain.Entity;
using Newcats.JobManager.Api.Infrastructure.DataAccess;
using Newcats.JobManager.Api.Infrastructure.DenpendencyInjection;

namespace Newcats.JobManager.Api.Domain.IService
{
    public interface IJobService : IScopeDependency
    {
        Task<bool> InsertJobInfoAsync(JobInfoEntity jobInfoEntity);

        Task<bool> UpdateJobStateAsync(int jobId, JobState jobState);

        Task<bool> UpdateJobAsync(int jobId, IEnumerable<DbUpdate<JobInfoEntity>> dbUpdates);

        Task<(IEnumerable<JobInfoEntity> list, int totalCount)> GetJobsAsync(int pageIndex, int pageSize, IEnumerable<DbWhere<JobInfoEntity>> dbWheres = null, int? commandTimeout = null, params DbOrderBy<JobInfoEntity>[] dbOrderBy);

        Task<JobInfoEntity> GetJobAsync(int jobId);

        /// <summary>
        /// 获取系统主Job(承载其他Job的系统级Job，唯一)
        /// </summary>
        /// <returns>唯一的系统主Job</returns>
        Task<JobInfoEntity> GetSystemMainJobAsync();

        /// <summary>
        /// 更新一个JobInfo记录
        /// </summary>
        /// <param name="jobInfoEntity">JobInfo记录</param>
        /// <returns>是否成功</returns>
        Task<bool> ModifyJobAsync(JobInfoEntity jobInfoEntity);

        /// <summary>
        /// 获取指定数量的最新的JobLog日志记录
        /// </summary>
        /// <param name="jobId">jobId</param>
        /// <param name="top">需要获取的数量</param>
        /// <returns>JobLog集合</returns>
        Task<IEnumerable<JobLogEntity>> GetLatestJobLogs(int jobId, int top);
    }
}