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
        private readonly IRepository<JobInfoEntity, int> _jobRepository;

        private readonly IRepository<JobLogEntity, long> _logRepository;

        public JobService()
        {
            _jobRepository = new Repository<JobInfoEntity, int>();
            _logRepository = new Repository<JobLogEntity, long>();
        }

        /// <summary>
        /// 获取允许进行调度的JobInfo
        /// Disabled=false&&JobState=Starting/Stopping/Updating/FireNow
        /// </summary>
        /// <returns>JobInfo集合</returns>
        public IEnumerable<JobInfoEntity> GetAllowScheduleJobs()
        {
            List<DbWhere<JobInfoEntity>> dbWheres = new List<DbWhere<JobInfoEntity>>
            {
                new DbWhere<JobInfoEntity>(j => j.Disabled, false),
                new DbWhere<JobInfoEntity>(j=>j.State,new int[]{Convert.ToInt32(JobState.Starting),Convert.ToInt32(JobState.Stopping),Convert.ToInt32(JobState.Updating),Convert.ToInt32(JobState.FireNow)}, OperateType.In)
            };
            return _jobRepository.GetAll(dbWheres, null, new DbOrderBy<JobInfoEntity>(j => j.CreateTime, SortType.ASC));
        }

        /// <summary>
        /// 更新Job状态
        /// </summary>
        /// <param name="jobId">主键</param>
        /// <param name="jobState">JobState</param>
        /// <returns>是否成功</returns>
        public bool UpdateJobState(int jobId, JobState jobState)
        {
            return _jobRepository.Update(jobId, new List<DbUpdate<JobInfoEntity>> { new DbUpdate<JobInfoEntity>(j => j.State, jobState) }) > 0;
        }

        /// <summary>
        /// 插入一条JobLog
        /// </summary>
        /// <param name="logEntity">JobLog实体</param>
        /// <returns>是否成功</returns>
        public bool InsertLog(JobLogEntity logEntity)
        {
            return _logRepository.Insert(logEntity) > 0;
        }

        /// <summary>
        /// 更新Job执行结果（插入日志，更新上次/下次执行时间，执行次数）（事务）
        /// </summary>
        /// <param name="nextFireTime">下次执行时间</param>
        /// <param name="logEntity">JobLog实体</param>
        /// <returns>是否成功</returns>
        public bool UpdateJobFireResult(DateTime nextFireTime, JobLogEntity logEntity)
        {
            int r = 0;
            using (TransactionScope trans = TransactionScopeBuilder.CreateReadCommitted(false))
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

        /// <summary>
        /// 插入一条JobInfo记录
        /// </summary>
        /// <param name="jobInfoEntity">JobInfo实体</param>
        /// <returns>主键Id</returns>
        public int InsertJob(JobInfoEntity jobInfoEntity)
        {
            return _jobRepository.Insert(jobInfoEntity);
        }

        /// <summary>
        /// 启动系统主Job
        /// </summary>
        /// <param name="systemJobId">主键</param>
        /// <returns>是否成功</returns>
        public bool SetSystemJobAvailable(int systemJobId)
        {
            List<DbUpdate<JobInfoEntity>> dbUpdates = new List<DbUpdate<JobInfoEntity>>
            {
                new DbUpdate<JobInfoEntity>(j => j.State, JobState.Starting),
                new DbUpdate<JobInfoEntity>(j => j.Disabled, false)
            };

            return _jobRepository.Update(systemJobId, dbUpdates) > 0;
        }

        /// <summary>
        /// 获取所有正在运行的未禁用Job
        /// </summary>
        /// <returns>JobInfo集合</returns>
        public IEnumerable<JobInfoEntity> GetAllRunningJobs()
        {
            List<DbWhere<JobInfoEntity>> dbWheres = new List<DbWhere<JobInfoEntity>>
            {
                new DbWhere<JobInfoEntity>(j => j.Disabled, false),
                new DbWhere<JobInfoEntity>(j=>j.State,JobState.Running)
            };
            return _jobRepository.GetAll(dbWheres, null, new DbOrderBy<JobInfoEntity>(j => j.CreateTime, SortType.ASC));
        }

        /// <summary>
        /// 把正在运行的Job状态改为Starting
        /// </summary>
        public void SetRunningJobStarting()
        {
            List<DbWhere<JobInfoEntity>> dbWheres = new List<DbWhere<JobInfoEntity>>
            {
                new DbWhere<JobInfoEntity>(j => j.Disabled, false),
                new DbWhere<JobInfoEntity>(j=>j.State,JobState.Running)
            };
            List<DbUpdate<JobInfoEntity>> dbUpdates = new List<DbUpdate<JobInfoEntity>>
            {
                new DbUpdate<JobInfoEntity>(j=>j.State,JobState.Starting)
            };

            _jobRepository.Update(dbWheres, dbUpdates);
        }
    }
}