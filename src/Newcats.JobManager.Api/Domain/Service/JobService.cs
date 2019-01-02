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
        private readonly IRepository<JobLogEntity, long> _logRepository;

        public JobService(IRepository<JobInfoEntity, int> job, IRepository<JobLogEntity, long> log)
        {
            _jobRepository = job;
            _logRepository = log;
        }

        public async Task<bool> InsertJobInfoAsync(JobInfoEntity jobInfoEntity)
        {
            return await _jobRepository.InsertAsync(jobInfoEntity) > 0;
        }

        public async Task<bool> UpdateJobStateAsync(int jobId, JobState jobState)
        {
            return await _jobRepository.UpdateAsync(jobId, new List<DbUpdate<JobInfoEntity>> { new DbUpdate<JobInfoEntity>(j => j.State, jobState) }) > 0;
        }

        public async Task<bool> UpdateJobAsync(int jobId, IEnumerable<DbUpdate<JobInfoEntity>> dbUpdates)
        {
            return await _jobRepository.UpdateAsync(jobId, dbUpdates) > 0;
        }

        public async Task<(IEnumerable<JobInfoEntity> list, int totalCount)> GetJobsAsync(int pageIndex, int pageSize, IEnumerable<DbWhere<JobInfoEntity>> dbWheres = null, int? commandTimeout = null, params DbOrderBy<JobInfoEntity>[] dbOrderBy)
        {
            return await _jobRepository.GetPageAsync(pageIndex, pageSize, dbWheres, commandTimeout, dbOrderBy);
        }

        public async Task<JobInfoEntity> GetJobAsync(int jobId)
        {
            return await _jobRepository.GetAsync(jobId);
        }

        /// <summary>
        /// 获取系统主Job(承载其他Job的系统级Job，唯一)
        /// </summary>
        /// <returns>唯一的系统主Job</returns>
        public async Task<JobInfoEntity> GetSystemMainJobAsync()
        {
            List<DbWhere<JobInfoEntity>> dbWheres = new List<DbWhere<JobInfoEntity>>
            {
                new DbWhere<JobInfoEntity>(j => j.JobLevel, JobLevel.System),
                new DbWhere<JobInfoEntity>(j => j.AssemblyName, "Newcats.JobManager.Host.exe"),
                new DbWhere<JobInfoEntity>(j => j.ClassName, "Newcats.JobManager.Host.Manager.SystemJob")
            };
            return await _jobRepository.GetAsync(dbWheres, dbOrderBy: new DbOrderBy<JobInfoEntity>(o => o.Id));
        }

        /// <summary>
        /// 更新一个JobInfo记录
        /// </summary>
        /// <param name="jobInfoEntity">JobInfo记录</param>
        /// <returns>是否成功</returns>
        public async Task<bool> ModifyJobAsync(JobInfoEntity jobInfoEntity)
        {
            List<DbUpdate<JobInfoEntity>> dbUpdates = new List<DbUpdate<JobInfoEntity>>
            {
                new DbUpdate<JobInfoEntity>(j=>j.JobLevel,jobInfoEntity.JobLevel),
                new DbUpdate<JobInfoEntity>(j=>j.Name,jobInfoEntity.Name),
                new DbUpdate<JobInfoEntity>(j=>j.Description,jobInfoEntity.Description),
                new DbUpdate<JobInfoEntity>(j=>j.AssemblyName,jobInfoEntity.AssemblyName),
                new DbUpdate<JobInfoEntity>(j=>j.ClassName,jobInfoEntity.ClassName),
                new DbUpdate<JobInfoEntity>(j=>j.JobArgs,jobInfoEntity.JobArgs),
                new DbUpdate<JobInfoEntity>(j=>j.CronExpression,jobInfoEntity.CronExpression),
                new DbUpdate<JobInfoEntity>(j=>j.CronExpressionDescription,jobInfoEntity.CronExpressionDescription),
                new DbUpdate<JobInfoEntity>(j=>j.UpdateTime,DateTime.Now)
            };
            return await _jobRepository.UpdateAsync(jobInfoEntity.Id, dbUpdates) > 0;
        }

        /// <summary>
        /// 获取指定数量的最新的JobLog日志记录
        /// </summary>
        /// <param name="top">需要获取的数量</param>
        /// <returns>JobLog集合</returns>
        public async Task<IEnumerable<JobLogEntity>> GetLatestJobLogs(int top)
        {
            return await _logRepository.GetTopAsync(top, dbOrderBy: new DbOrderBy<JobLogEntity>(j => j.CreateTime, SortType.DESC));
        }
    }
}