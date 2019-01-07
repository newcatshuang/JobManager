using System;
using Newcats.JobManager.Api.Domain.Entity;

namespace Newcats.JobManager.Api.Models.Requests
{
    public class LogListRequest : PageRequest
    {
        /// <summary>
        /// LogId(主键/自增)
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// JobId(主键/自增)
        /// </summary>
        public int? JobId { get; set; }

        /// <summary>
        /// Job名称
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime? FireTimeStart { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime? FireTimeEnd { get; set; }

        /// <summary>
        /// 执行结果
        /// </summary>
        public FireState? FireState { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTimeStart { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTimeEnd { get; set; }
    }
}