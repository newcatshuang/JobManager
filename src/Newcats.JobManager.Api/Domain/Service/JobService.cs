using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newcats.JobManager.Api.Domain.Entity;
using Newcats.JobManager.Api.Domain.IService;
using Newcats.JobManager.Api.Infrastructure.DataAccess;

namespace Newcats.JobManager.Api.Domain.Service
{
    public class JobService : IJobService
    {
        private readonly IRepository<JobInfoEntity, int> _jobRepository;
        private readonly IRepository<JobLogEntity, long> _logepository;

        public JobService(IRepository<JobInfoEntity, int> job, IRepository<JobLogEntity, long> log)
        {
            _jobRepository = job;
            _logepository = log;
        }

        public async Task<bool> InsertJobInfoAsync(JobInfoEntity jobInfoEntity)
        {
            return await _jobRepository.InsertAsync(jobInfoEntity) > 0;
        }

        public async Task<bool> UpdateJobStateAsync(int jobId, JobState jobState)
        {
            return await _jobRepository.UpdateAsync(jobId, new List<DbUpdate<JobInfoEntity>> { new DbUpdate<JobInfoEntity>(j => j.State, jobState) }) > 0;
        }

        public async Task<(IEnumerable<JobInfoEntity> list, int totalCount)> GetJobsAsync(int pageIndex, int pageSize, IEnumerable<DbWhere<JobInfoEntity>> dbWheres = null, int? commandTimeout = null, params DbOrderBy<JobInfoEntity>[] dbOrderBy)
        {
            return await _jobRepository.GetPageAsync(pageIndex, pageSize, dbWheres, commandTimeout, dbOrderBy);
        }
    }
}
