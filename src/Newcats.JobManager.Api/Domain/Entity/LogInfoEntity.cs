using System;
using Newcats.JobManager.Api.Infrastructure.DataAccess;

namespace Newcats.JobManager.Api.Domain.Entity
{
    [TableName(" JobLog a with(nolock) left join JobInfo b with(nolock) on a.JobId=b.Id ")]
    public class LogInfoEntity : IEntity
    {
        /// <summary>
        /// LogId(主键/自增)
        /// </summary>
        [RealColumn("a.Id")]
        public long Id { get; set; }

        /// <summary>
        /// JobId(主键/自增)
        /// </summary>
        [RealColumn("a.JobId")]
        public int JobId { get; set; }

        /// <summary>
        /// Job名称
        /// </summary>
        [RealColumn("b.Name")]
        public string JobName { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        [RealColumn("a.FireTime")]
        public DateTime? FireTime { get; set; }

        /// <summary>
        /// 执行耗时
        /// </summary>
        [RealColumn("a.FireDuration")]
        public decimal? FireDuration { get; set; }

        /// <summary>
        /// 执行结果
        /// </summary>
        [RealColumn("a.FireState")]
        public FireState FireState { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        [RealColumn("a.Content")]
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [RealColumn("a.CreateTime")]
        public DateTime? CreateTime { get; set; }
    }
}