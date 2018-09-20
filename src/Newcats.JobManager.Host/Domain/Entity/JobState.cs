namespace Newcats.JobManager.Host.Domain.Entity
{
    public enum JobState
    {
        /// <summary>
        /// 停止
        /// </summary>
        Stop = 0,

        /// <summary>
        /// 运行
        /// </summary>
        Running = 1,

        /// <summary>
        /// 启动中
        /// </summary>
        Starting = 3,

        /// <summary>
        /// 停止中
        /// </summary>
        Stopping = 5
    }
}