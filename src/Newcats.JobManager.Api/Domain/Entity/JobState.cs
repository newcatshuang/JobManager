namespace Newcats.JobManager.Api.Domain.Entity
{
    public enum JobState
    {
        /// <summary>
        /// 停止
        /// </summary>
        Stopped = 0,

        /// <summary>
        /// 运行中
        /// </summary>
        Running = 1,

        /// <summary>
        /// 启动中
        /// </summary>
        Starting = 3,

        /// <summary>
        /// 停止中
        /// </summary>
        Stopping = 5,

        /// <summary>
        /// 等待更新
        /// </summary>
        Updating = 7,

        /// <summary>
        /// 等待立即执行一次
        /// </summary>
        FireNow = 9
    }
}