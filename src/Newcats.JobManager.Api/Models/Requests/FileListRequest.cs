using System;

namespace Newcats.JobManager.Api.Models.Requests
{
    /// <summary>
    /// 文件选项卡请求参数
    /// </summary>
    public class FileListRequest : PageRequest
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTimeStart { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTimeEnd { get; set; }

        /// <summary>
        /// 访问时间
        /// </summary>
        public DateTime? AccessTimeStart { get; set; }

        /// <summary>
        /// 访问时间
        /// </summary>
        public DateTime? AccessTimeEnd { get; set; }

        /// <summary>
        /// 写入时间
        /// </summary>
        public DateTime? WriteTimeStart { get; set; }

        /// <summary>
        /// 写入时间
        /// </summary>
        public DateTime? WriteTimeEnd { get; set; }

        /// <summary>
        /// 是否显示系统程序集
        /// </summary>
        public bool ShowDefaultDLL { get; set; }
    }
}