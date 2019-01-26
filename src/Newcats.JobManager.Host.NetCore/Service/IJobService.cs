using System;
using System.Collections.Generic;
using Newcats.JobManager.Common.NetCore.Entity;

namespace Newcats.JobManager.Host.NetCore.Service
{
    public interface IJobService
    {
        IEnumerable<JobInfoEntity> GetAllowScheduleJobs();

        bool UpdateJobState(int jobId, JobState jobState);

        bool InsertLog(JobLogEntity logEntity);

        bool UpdateJobFireResult(DateTime nextFireTime, JobLogEntity logEntity);

        /// <summary>
        /// 获取系统主Job(承载其他Job的系统级Job，唯一)
        /// </summary>
        /// <returns>唯一的系统主Job</returns>
        JobInfoEntity GetSystemMainJobAsync();

        int InsertJob(JobInfoEntity jobInfoEntity);

        bool SetSystemJobAvailable(int systemJobId);
    }
}