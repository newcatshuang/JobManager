using System.ComponentModel;

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
        [Description("成功")]
        Success = 0,

        /// <summary>
        /// 失败
        /// </summary>
        [Description("失败")]
        Failed = 1,

        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        Error = 2
    }
}