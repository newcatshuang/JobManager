using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newcats.JobManager.Api.Domain.Entity;
using Newcats.JobManager.Api.Domain.IService;
using Newcats.JobManager.Common.DataAccess;
using Newcats.JobManager.Common.Entity;

namespace Newcats.JobManager.Api.Domain.Service
{
    public class JobService : IJobService
    {
        private readonly IRepository<JobInfoEntity, int> _jobRepository;
        private readonly IRepository<JobLogEntity, long> _logRepository;
        private readonly IRepository<LogInfoEntity, long> _logInfoRepository;

        public JobService(IRepository<JobInfoEntity, int> job, IRepository<JobLogEntity, long> log, IRepository<LogInfoEntity, long> logInfo)
        {
            _jobRepository = job;
            _logRepository = log;
            _logInfoRepository = logInfo;
        }

        /// <summary>
        /// 插入一条JonInfo记录
        /// </summary>
        /// <param name="jobInfoEntity">JonInfo记录</param>
        /// <returns>是否成功</returns>
        public async Task<bool> InsertJobInfoAsync(JobInfoEntity jobInfoEntity)
        {
            return await _jobRepository.InsertAsync(jobInfoEntity) > 0;
        }

        /// <summary>
        /// 根据主键，更新Job状态
        /// </summary>
        /// <param name="jobId">主键Id</param>
        /// <param name="jobState">状态</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateJobStateAsync(int jobId, JobState jobState)
        {
            return await _jobRepository.UpdateAsync(jobId, new List<DbUpdate<JobInfoEntity>> { new DbUpdate<JobInfoEntity>(j => j.State, jobState) }) > 0;
        }

        /// <summary>
        /// 根据主键，更新一条JonInfo记录
        /// </summary>
        /// <param name="jobId">主键</param>
        /// <param name="dbUpdates">要更新的数据</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateJobAsync(int jobId, IEnumerable<DbUpdate<JobInfoEntity>> dbUpdates)
        {
            return await _jobRepository.UpdateAsync(jobId, dbUpdates) > 0;
        }

        /// <summary>
        /// 分页获取JobInfo实体集合
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="dbWheres">筛选条件</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="dbOrderBy">排序条件</param>
        /// <returns>JobInfo实体集合</returns>
        public async Task<(IEnumerable<JobInfoEntity> list, int totalCount)> GetJobsAsync(int pageIndex, int pageSize, IEnumerable<DbWhere<JobInfoEntity>> dbWheres = null, int? commandTimeout = null, params DbOrderBy<JobInfoEntity>[] dbOrderBy)
        {
            return await _jobRepository.GetPageAsync(pageIndex, pageSize, dbWheres, commandTimeout, dbOrderBy);
        }

        /// <summary>
        /// 根据主键Id,获取一个JobInfo实体
        /// </summary>
        /// <param name="jobId">主键</param>
        /// <returns>JobInfo实体</returns>
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
        /// <param name="jobId">jobId</param>
        /// <returns>JobLog集合</returns>
        public async Task<IEnumerable<JobLogEntity>> GetLatestJobLogs(int jobId, int top)
        {
            return await _logRepository.GetTopAsync(top, dbWheres: new List<DbWhere<JobLogEntity>>() { new DbWhere<JobLogEntity>(j => j.JobId, jobId) }, dbOrderBy: new DbOrderBy<JobLogEntity>(j => j.CreateTime, SortType.DESC));
        }

        /// <summary>
        /// 分页获取JobLog实体集合
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="dbWheres">筛选条件</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="dbOrderBy">排序条件</param>
        /// <returns>JobLog实体集合</returns>
        public async Task<(IEnumerable<LogInfoEntity> list, int totalCount)> GetLogsAsync(int pageIndex, int pageSize, IEnumerable<DbWhere<LogInfoEntity>> dbWheres = null, int? commandTimeout = null, params DbOrderBy<LogInfoEntity>[] dbOrderBy)
        {
            return await _logInfoRepository.GetPageAsync(pageIndex, pageSize, dbWheres, commandTimeout, dbOrderBy);
        }
    }
}