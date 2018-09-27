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

        Task<(IEnumerable<JobInfoEntity> list, int totalCount)> GetJobsAsync(int pageIndex, int pageSize, IEnumerable<DbWhere<JobInfoEntity>> dbWheres = null, int? commandTimeout = null, params DbOrderBy<JobInfoEntity>[] dbOrderBy);

        Task<JobInfoEntity> GetJobAsync(int jobId);
    }
}