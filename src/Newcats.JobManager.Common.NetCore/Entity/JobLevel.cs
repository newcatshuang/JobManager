using System.ComponentModel;

namespace Newcats.JobManager.Common.NetCore.Entity
{
    /// <summary>
    /// Job等级
    /// </summary>
    public enum JobLevel
    {
        /// <summary>
        /// 业务
        /// </summary>
        [Description("业务")]
        Business = 0,

        /// <summary>
        /// 测试
        /// </summary>
        [Description("测试")]
        Test = 1,

        /// <summary>
        /// 系统
        /// </summary>
        [Description("系统")]
        System = 2
    }
}