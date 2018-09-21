using System;
using System.Collections.Generic;
using System.Linq;
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

        public static bool AddJob(JobInfoEntity jobInfo)
        {
            jobInfo.CreateTime = DateTime.Now;
            jobInfo.UpdateTime = DateTime.Now;
            return _jobRepository.Insert(jobInfo) > 0;
        }

        public static bool ModifyJob(JobInfoEntity jobInfo)
        {
            List<DbUpdate<JobInfoEntity>> dbUpdates = new List<DbUpdate<JobInfoEntity>>
            {
                new DbUpdate<JobInfoEntity>(j => j.JobLevel, jobInfo.JobLevel),
                new DbUpdate<JobInfoEntity>(j => j.Name, jobInfo.Name),
                new DbUpdate<JobInfoEntity>(j => j.AssemblyName, jobInfo.AssemblyName),
                new DbUpdate<JobInfoEntity>(j => j.ClassName, jobInfo.ClassName),
                new DbUpdate<JobInfoEntity>(j => j.Description, jobInfo.Description),
                new DbUpdate<JobInfoEntity>(j => j.JobArgs, jobInfo.JobArgs),
                new DbUpdate<JobInfoEntity>(j => j.CronExpression, jobInfo.CronExpression),
                new DbUpdate<JobInfoEntity>(j => j.CronExpressionDescription, jobInfo.CronExpressionDescription),
                new DbUpdate<JobInfoEntity>(j => j.State, jobInfo.State),
                new DbUpdate<JobInfoEntity>(j => j.UpdateId, jobInfo.UpdateId),
                new DbUpdate<JobInfoEntity>(j => j.UpdateName, jobInfo.UpdateName),
                new DbUpdate<JobInfoEntity>(j => j.UpdateTime, jobInfo.UpdateTime)
            };
            return _jobRepository.Update(jobInfo.Id, dbUpdates) > 0;
        }

        public static bool RemoveJob(long jobId)
        {
            List<DbUpdate<JobInfoEntity>> dbUpdates = new List<DbUpdate<JobInfoEntity>>
            {
                new DbUpdate<JobInfoEntity>(j => j.Disabled, true),
                new DbUpdate<JobInfoEntity>(j => j.UpdateTime, DateTime.Now)
            };
            return _jobRepository.Update(jobId, dbUpdates) > 0;
        }

        public static int RemoveJob(IEnumerable<long> jobIds)
        {
            int i = 0;
            if (jobIds != null && jobIds.Any())
            {
                List<DbUpdate<JobInfoEntity>> dbUpdates = new List<DbUpdate<JobInfoEntity>>
                {
                    new DbUpdate<JobInfoEntity>(j => j.Disabled, true),
                    new DbUpdate<JobInfoEntity>(j => j.UpdateTime, DateTime.Now)
                };

                List<DbWhere<JobInfoEntity>> dbWheres = new List<DbWhere<JobInfoEntity>>
                {
                    new DbWhere<JobInfoEntity>(j => j.State, JobState.Stopped),
                    new DbWhere<JobInfoEntity>(j => j.Id, jobIds, OperateType.In)
                };
                i = _jobRepository.Update(dbWheres, dbUpdates);
            }
            return i;
        }

        public static JobInfoEntity GetJob(long jobId)
        {
            return _jobRepository.Get(jobId);
        }

        public static IEnumerable<JobInfoEntity> GetJobs(int pageIndex, int pageSize, ref int totalCount, IEnumerable<DbWhere<JobInfoEntity>> dbWheres = null, int? commandTimeout = null, params DbOrderBy<JobInfoEntity>[] dbOrderBy)
        {
            return _jobRepository.GetPage(pageIndex, pageSize, ref totalCount, dbWheres, commandTimeout, dbOrderBy);
        }

        public static IEnumerable<JobInfoEntity> GetAllowScheduleJobs()
        {
            List<DbWhere<JobInfoEntity>> dbWheres = new List<DbWhere<JobInfoEntity>>
            {
                new DbWhere<JobInfoEntity>(j => j.Disabled, false),
                new DbWhere<JobInfoEntity>(j => j.State, JobState.Stopped, OperateType.NotEqual)
            };
            return _jobRepository.GetAll(dbWheres, null, new DbOrderBy<JobInfoEntity>(j => j.CreateTime, SortType.DESC));
        }

        public static bool UpdateJobState(long jobId, JobState jobState)
        {
            return _jobRepository.Update(jobId, new List<DbUpdate<JobInfoEntity>> { new DbUpdate<JobInfoEntity>(j => j.State, jobState) }) > 0;
        }

        public static bool UpdateJobStatus(long jobId, DateTime lastFireTime, DateTime NextFireTime)
        {
            JobInfoEntity job = _jobRepository.Get(jobId);
            List<DbUpdate<JobInfoEntity>> dbUpdates = new List<DbUpdate<JobInfoEntity>>
            {
                new DbUpdate<JobInfoEntity>(j => j.LastFireTime, lastFireTime),
                new DbUpdate<JobInfoEntity>(j => j.NextFireTime, NextFireTime),
                new DbUpdate<JobInfoEntity>(j => j.FireCount, job.FireCount + 1)
            };
            return _jobRepository.Update(jobId, dbUpdates) > 0;
        }

        public static bool UpdateJobStatus(long jobId, DateTime lastFireTime, DateTime nextFireTime, double executionDuration, string content)
        {
            bool isSuccess = false;
            using (TransactionScope trans = new TransactionScope())
            {
                isSuccess = UpdateJobStatus(jobId, lastFireTime, nextFireTime);
                JobLogEntity log = new JobLogEntity
                {
                    JobId = jobId,
                    CreateTime = DateTime.Now,
                    FireDuration = executionDuration,
                    FireTime = lastFireTime,
                    Content = content
                };
                isSuccess = AddLog(log);
                if (isSuccess)
                    trans.Complete();
            }
            return isSuccess;
        }

        public static bool AddLog(long jobId, DateTime executionTime, string contnet)
        {
            return AddLog(new JobLogEntity
            {
                JobId = jobId,
                CreateTime = DateTime.Now,
                FireTime = executionTime,
                FireDuration = 0,
                Content = contnet
            });
        }

        public static bool AddLog(JobLogEntity jobLog)
        {
            return _logRepository.Insert(jobLog) > 0;
        }

        public static JobLogEntity GetLog(long logId)
        {
            return _logRepository.Get(logId);
        }

        public static IEnumerable<JobLogEntity> GetLogs(int pageIndex, int pageSize, ref int totalCount, IEnumerable<DbWhere<JobLogEntity>> dbWheres = null, int? commandTimeout = null, params DbOrderBy<JobLogEntity>[] dbOrderBy)
        {
            return _logRepository.GetPage(pageIndex, pageSize, ref totalCount, dbWheres, commandTimeout, dbOrderBy);
        }

        public static bool DeleteLog(long logId)
        {
            return _logRepository.Delete(logId) > 0;
        }

        public static int DeleteLogs(IEnumerable<long> logIds)
        {
            if (logIds != null && logIds.Any())
            {
                return _logRepository.Delete(new List<DbWhere<JobLogEntity>> { new DbWhere<JobLogEntity>(l => l.Id, logIds, OperateType.In) });
            }
            return 0;
        }
    }
}