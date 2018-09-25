using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Newcats.JobManager.Host.Domain.Entity;
using Newcats.JobManager.Host.Domain.Repository;

namespace Newcats.JobManager.Host.Domain.Service
{
    public class JobService
    {
        private static readonly Repository<JobInfoEntity, long> _jobRepository;

        private static readonly Repository<JobLogEntity, long> _logRepository;

        static JobService()
        {
            _jobRepository = new Repository<JobInfoEntity, long>();
            _logRepository = new Repository<JobLogEntity, long>();
        }

        #region 异步方法(使用异步方法后台job执行有bug)
        public static async Task<IEnumerable<JobInfoEntity>> GetAllowScheduleJobsAsync()
        {
            List<DbWhere<JobInfoEntity>> dbWheres = new List<DbWhere<JobInfoEntity>>
            {
                new DbWhere<JobInfoEntity>(j => j.Disabled, false),
                new DbWhere<JobInfoEntity>(j=>j.State,new int[]{Convert.ToInt32(JobState.Starting),Convert.ToInt32(JobState.Stopping),Convert.ToInt32(JobState.Updating),Convert.ToInt32(JobState.FireNow)}, OperateType.In)
            };
            return await _jobRepository.GetAllAsync(dbWheres, null, new DbOrderBy<JobInfoEntity>(j => j.CreateTime, SortType.ASC));
        }

        public static async Task<bool> UpdateJobStateAsync(long jobId, JobState jobState)
        {
            return await _jobRepository.UpdateAsync(jobId, new List<DbUpdate<JobInfoEntity>> { new DbUpdate<JobInfoEntity>(j => j.State, jobState) }) > 0;
        }

        public static async Task<bool> InsertLogAsync(JobLogEntity logEntity)
        {
            return await _logRepository.InsertAsync(logEntity) > 0;
        }

        public static async Task<bool> UpdateJobFireResultAsync(DateTime nextFireTime, JobLogEntity logEntity)
        {
            int r = 0;
            using (TransactionScope trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                JobInfoEntity job = await _jobRepository.GetAsync(logEntity.JobId);
                r = await _jobRepository.UpdateAsync(logEntity.JobId, new List<DbUpdate<JobInfoEntity>>
                {
                    new DbUpdate<JobInfoEntity>(j=>j.LastFireTime,logEntity.FireTime),
                    new DbUpdate<JobInfoEntity>(j=>j.NextFireTime,nextFireTime),
                    new DbUpdate<JobInfoEntity>(j=>j.FireCount,job.FireCount+1)
                });
                r += await _logRepository.InsertAsync(logEntity) > 0 ? 1 : 0;
                if (r >= 2)
                    trans.Complete();
            }
            return r >= 2;
        }
        #endregion

        public static IEnumerable<JobInfoEntity> GetAllowScheduleJobs()
        {
            List<DbWhere<JobInfoEntity>> dbWheres = new List<DbWhere<JobInfoEntity>>
            {
                new DbWhere<JobInfoEntity>(j => j.Disabled, false),
                new DbWhere<JobInfoEntity>(j=>j.State,new int[]{Convert.ToInt32(JobState.Starting),Convert.ToInt32(JobState.Stopping),Convert.ToInt32(JobState.Updating),Convert.ToInt32(JobState.FireNow)}, OperateType.In)
            };
            return _jobRepository.GetAll(dbWheres, null, new DbOrderBy<JobInfoEntity>(j => j.CreateTime, SortType.ASC));
        }

        public static bool UpdateJobState(long jobId, JobState jobState)
        {
            return _jobRepository.Update(jobId, new List<DbUpdate<JobInfoEntity>> { new DbUpdate<JobInfoEntity>(j => j.State, jobState) }) > 0;
        }

        public static bool InsertLog(JobLogEntity logEntity)
        {
            return _logRepository.Insert(logEntity) > 0;
        }

        public static bool UpdateJobFireResult(DateTime nextFireTime, JobLogEntity logEntity)
        {
            int r = 0;
            using (TransactionScope trans = new TransactionScope())
            {
                JobInfoEntity job = _jobRepository.Get(logEntity.JobId);
                r = _jobRepository.Update(logEntity.JobId, new List<DbUpdate<JobInfoEntity>>
                {
                    new DbUpdate<JobInfoEntity>(j=>j.LastFireTime,logEntity.FireTime),
                    new DbUpdate<JobInfoEntity>(j=>j.NextFireTime,nextFireTime),
                    new DbUpdate<JobInfoEntity>(j=>j.FireCount,job.FireCount+1)
                });
                r += _logRepository.Insert(logEntity) > 0 ? 1 : 0;
                if (r >= 2)
                    trans.Complete();
            }
            return r >= 2;
        }
    }
}