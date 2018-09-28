using System;
using Newcats.JobManager.Api.Domain.Entity;

namespace Newcats.JobManager.Api.Models.Requests
{
    public class JobListRequest : PageRequest
    {
        /// <summary>
        /// JobId(主键/自增)
        /// </summary>				
        public int? Id { get; set; }

        /// <summary>
        /// Job名称
        /// </summary>				
        public string Name { get; set; }

        /// <summary>
        /// Job等级(0.业务，1.测试，2.系统)
        /// </summary>
        public JobLevel? JobLevel { get; set; }

        /// <summary>
        /// 程序集名称(所属程序集)(例:Newcats.JobManager.Host.exe)
        /// </summary>				
        public string AssemblyName { get; set; }

        /// <summary>
        /// 类名(完整命名空间的类名)(例:Newcats.JobManager.Host.Manager.SystemJob)
        /// </summary>				
        public string ClassName { get; set; }

        /// <summary>
        /// Cron表达式
        /// </summary>				
        public string CronExpression { get; set; }

        /// <summary>
        /// 上次运行时间
        /// </summary>				
        public DateTime? LastFireTimeStart { get; set; }

        /// <summary>
        /// 上次运行时间
        /// </summary>				
        public DateTime? LastFireTimeEnd { get; set; }

        /// <summary>
        /// 下次运行时间
        /// </summary>				
        public DateTime? NextFireTimeStart { get; set; }

        /// <summary>
        /// 下次运行时间
        /// </summary>				
        public DateTime? NextFireTimeEnd { get; set; }

        /// <summary>
        /// 状态(0.停止，1.运行中，3.启动中，5.停止中，7.等待更新，9.等待立即执行一次)
        /// </summary>
        public JobState? State { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>				
        public bool? Disabled { get; set; }
    }
}