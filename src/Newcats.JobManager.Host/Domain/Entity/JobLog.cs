using System;
using Newcats.JobManager.Common.DataAccess;

namespace Newcats.JobManager.Host.Domain.Entity
{
    /// <summary>
    /// Job执行日志
    /// </summary>
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
        public int JobId { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>				
        public DateTime? FireTime { get; set; }

        /// <summary>
        /// 执行持续时长
        /// </summary>				
        public double? FireDuration { get; set; }

        /// <summary>
        /// 执行结果(0.成功，1.失败，2.异常)
        /// </summary>
        public FireState FireState { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>				
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>				
        public DateTime? CreateTime { get; set; }
    }
}