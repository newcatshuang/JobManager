using System.ComponentModel;

namespace Newcats.JobManager.Host.Domain.Entity
{
    public enum JobState
    {
        /// <summary>
        /// 停止
        /// </summary>
        [Description("停止")]
        Stopped = 0,

        /// <summary>
        /// 运行中
        /// </summary>
        [Description("运行中")]
        Running = 1,

        /// <summary>
        /// 启动中
        /// </summary>
        [Description("启动中")]
        Starting = 3,

        /// <summary>
        /// 停止中
        /// </summary>
        [Description("停止中")]
        Stopping = 5,

        /// <summary>
        /// 等待更新
        /// </summary>
        [Description("等待更新")]
        Updating = 7,

        /// <summary>
        /// 等待立即执行
        /// </summary>
        [Description("等待立即执行")]
        FireNow = 9
    }
}