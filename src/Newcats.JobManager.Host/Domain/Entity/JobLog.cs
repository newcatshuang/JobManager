using System;
using Newcats.JobManager.Host.Domain.Repository;

namespace Newcats.JobManager.Host.Domain.Entity
{
    [TableName("JobLog")]
    public class JobLogEntity : IEntity
    {
        /// <summary>
        /// LogId(主键/自增)
        /// </summary>				
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }

        /// <summary>
        /// JobId(表JobInfo.Id)
        /// </summary>
        public long JobId { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>				
        public DateTime? ExecutionTime { get; set; }

        /// <summary>
        /// 执行持续时长
        /// </summary>				
        public double? ExecutionDuration { get; set; }

        /// <summary>
        /// 创建日期时间
        /// </summary>				
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>				
        public string RunLog { get; set; }
    }
}