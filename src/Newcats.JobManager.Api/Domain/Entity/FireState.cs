namespace Newcats.JobManager.Api.Domain.Entity
{
    /// <summary>
    /// Job执行结果
    /// </summary>
    public enum FireState
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 失败
        /// </summary>
        Failed = 1,

        /// <summary>
        /// 异常
        /// </summary>
        Error = 2
    }
}