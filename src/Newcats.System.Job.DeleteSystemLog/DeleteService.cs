using System;
using System.Collections.Generic;
using System.Linq;
using Newcats.JobManager.Common.NetCore.DataAccess;
using Newcats.JobManager.Common.NetCore.Entity;

namespace Newcats.System.Job.DeleteSystemLog
{
    public class DeleteService
    {
        private readonly Repository<JobLogEntity, long> _logRepository;
        private readonly Repository<JobInfoEntity, int> _jobRepository;

        public DeleteService()
        {
            _logRepository = new Repository<JobLogEntity, long>();
            _jobRepository = new Repository<JobInfoEntity, int>();
        }

        public bool DeleteSystemLog()
        {
            IEnumerable<JobInfoEntity> jobs = _jobRepository.GetAll(new List<DbWhere<JobInfoEntity>>()
            {
                new DbWhere<JobInfoEntity>(j=>j.JobLevel,JobLevel.System)
            });
            return _logRepository.Delete(new List<DbWhere<JobLogEntity>>
            {
                new DbWhere<JobLogEntity>(j => j.CreateTime, DateTime.Now.AddMonths(-1), OperateType.LessEqual),
                new DbWhere<JobLogEntity>(j=>j.JobId,jobs.Select(s=>s.Id).ToArray(), OperateType.In)
            }, commandTimeout: 600) > 0;
        }
    }
}