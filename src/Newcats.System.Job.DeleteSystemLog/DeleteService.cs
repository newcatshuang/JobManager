using System;
using System.Collections.Generic;
using Newcats.JobManager.Common.DataAccess;
using Newcats.JobManager.Common.Entity;

namespace Newcats.System.Job.DeleteSystemLog
{
    public class DeleteService
    {
        private readonly Repository<JobLogEntity, long> _logRepository;

        public DeleteService()
        {
            _logRepository = new Repository<JobLogEntity, long>();
        }

        /// <summary>
        /// 删除一个月前的JobLevel.System级的JobLog
        /// </summary>
        /// <returns>是否成功</returns>
        public bool DeleteSystemLog()
        {
            //写法1
            return _logRepository.Delete(new List<DbWhere<JobLogEntity>>
            {
                new DbWhere<JobLogEntity>(j => j.CreateTime, DateTime.Now.AddMonths(-1), OperateType.LessEqual),
                new DbWhere<JobLogEntity>(j=>j.Id,$" (JobId in (SELECT Id FROM dbo.JobInfo WHERE JobLevel={(int)JobLevel.System})) ", OperateType.SqlText)
            }, commandTimeout: 600) > 0;

            //写法2
            //string sql = "DELETE FROM dbo.JobLog where JobId in (SELECT Id FROM dbo.JobInfo WHERE JobLevel=@JobLevel) and CreateTime<=@CreateTime;";
            //Dapper.DynamicParameters dp = new Dapper.DynamicParameters();
            //dp.Add("@JobLevel", JobLevel.System);
            //dp.Add("@CreateTime", DateTime.Now.AddMonths(-1));
            //return _logRepository.Execute(sql, dp, 600) > 0;
        }
    }
}