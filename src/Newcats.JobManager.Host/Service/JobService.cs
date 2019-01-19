using System;
using System.Collections.Generic;
using System.Reflection;
using System.Transactions;
using Newcats.JobManager.Common.DataAccess;
using Newcats.JobManager.Common.Entity;

namespace Newcats.JobManager.Host.Service
{
    public class JobService
    {
        private readonly Repository<JobInfoEntity, int> _jobRepository;

        private readonly Repository<JobLogEntity, long> _logRepository;

        public JobService()
        {
            _jobRepository = new Repository<JobInfoEntity, int>();
            _logRepository = new Repository<JobLogEntity, long>();
        }

        public IEnumerable<JobInfoEntity> GetAllowScheduleJobs()
        {
            List<DbWhere<JobInfoEntity>> dbWheres = new List<DbWhere<JobInfoEntity>>
            {
                new DbWhere<JobInfoEntity>(j => j.Disabled, false),
                new DbWhere<JobInfoEntity>(j=>j.State,new int[]{Convert.ToInt32(JobState.Starting),Convert.ToInt32(JobState.Stopping),Convert.ToInt32(JobState.Updating),Convert.ToInt32(JobState.FireNow)}, OperateType.In)
            };
            return _jobRepository.GetAll(dbWheres, null, new DbOrderBy<JobInfoEntity>(j => j.CreateTime, SortType.ASC));
        }

        public bool UpdateJobState(int jobId, JobState jobState)
        {
            return _jobRepository.Update(jobId, new List<DbUpdate<JobInfoEntity>> { new DbUpdate<JobInfoEntity>(j => j.State, jobState) }) > 0;
        }

        public bool InsertLog(JobLogEntity logEntity)
        {
            return _logRepository.Insert(logEntity) > 0;
        }

        public bool UpdateJobFireResult(DateTime nextFireTime, JobLogEntity logEntity)
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

        /// <summary>
        /// 获取系统主Job(承载其他Job的系统级Job，唯一)
        /// </summary>
        /// <returns>唯一的系统主Job</returns>
        public JobInfoEntity GetSystemMainJobAsync()
        {
            List<DbWhere<JobInfoEntity>> dbWheres = new List<DbWhere<JobInfoEntity>>
            {
                new DbWhere<JobInfoEntity>(j => j.JobLevel, JobLevel.System),
                new DbWhere<JobInfoEntity>(j => j.AssemblyName, Assembly.GetExecutingAssembly().ManifestModule.Name),//Newcats.JobManager.Host.exe
                new DbWhere<JobInfoEntity>(j => j.ClassName, typeof(Manager.SystemJob).FullName)//Newcats.JobManager.Host.Manager.SystemJob
            };

            return _jobRepository.Get(dbWheres, dbOrderBy: new DbOrderBy<JobInfoEntity>(j => j.Id));
        }

        public int InsertJob(JobInfoEntity jobInfoEntity)
        {
            return _jobRepository.Insert(jobInfoEntity);
        }

        public bool SetSystemJobAvailable(int systemJobId)
        {
            List<DbUpdate<JobInfoEntity>> dbUpdates = new List<DbUpdate<JobInfoEntity>>
            {
                new DbUpdate<JobInfoEntity>(j => j.State, JobState.Starting),
                new DbUpdate<JobInfoEntity>(j => j.Disabled, false)
            };

            return _jobRepository.Update(systemJobId, dbUpdates) > 0;
        }
    }
}