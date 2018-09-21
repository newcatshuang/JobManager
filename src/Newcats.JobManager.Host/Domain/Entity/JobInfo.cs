using System;
using Newcats.JobManager.Host.Domain.Repository;

namespace Newcats.JobManager.Host.Domain.Entity
{
    /// <summary>
    /// Job信息
    /// </summary>
    [TableName("JobInfo")]
    public class JobInfoEntity : IEntity
    {
        /// <summary>
        /// JobId(主键/自增)
        /// </summary>				
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }

        /// <summary>
        /// Job等级(0.业务，1.测试，2.系统)
        /// </summary>
        public JobLevel JobLevel { get; set; }

        /// <summary>
        /// Job名称
        /// </summary>				
        public string Name { get; set; }

        /// <summary>
        /// Job描述
        /// </summary>				
        public string Description { get; set; }

        /// <summary>
        /// 程序集名称(所属程序集)
        /// </summary>				
        public string AssemblyName { get; set; }

        /// <summary>
        /// 类名(完整命名空间的类名)
        /// </summary>				
        public string ClassName { get; set; }

        /// <summary>
        /// 参数
        /// </summary>				
        public string JobArgs { get; set; }

        /// <summary>
        /// Cron表达式
        /// </summary>				
        public string CronExpression { get; set; }

        /// <summary>
        /// Cron表达式描述
        /// </summary>				
        public string CronExpressionDescription { get; set; }

        /// <summary>
        /// 上次运行时间
        /// </summary>				
        public DateTime? LastFireTime { get; set; }

        /// <summary>
        /// 下次运行时间
        /// </summary>				
        public DateTime? NextFireTime { get; set; }

        /// <summary>
        /// 运行次数
        /// </summary>
        public int FireCount { get; set; }

        /// <summary>
        /// 状态(0.停止，1.运行，3.启动中，5.停止中)
        /// </summary>
        public JobState State { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>				
        public long? CreateId { get; set; }

        /// <summary>
        /// 创建人姓名
        /// </summary>				
        public string CreateName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>				
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 最后更新人ID
        /// </summary>				
        public long? UpdateId { get; set; }

        /// <summary>
        /// 最后更新人姓名
        /// </summary>				
        public string UpdateName { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>				
        public bool Disabled { get; set; }
    }
}