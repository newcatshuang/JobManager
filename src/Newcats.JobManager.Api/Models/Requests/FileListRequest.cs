using System;

namespace Newcats.JobManager.Api.Models.Requests
{
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
    }
}