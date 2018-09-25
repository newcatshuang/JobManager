using System;

namespace Newcats.JobManager.Api.Models.Requests
{
    /// <summary>
    /// 前端插件datatables.js的基本请求值
    /// </summary>
    public class PageRequest
    {
        /// <summary>
        /// 本次会话的请求次数
        /// </summary>
        public int Draw { get; set; }

        /// <summary>
        /// 跳过的数量
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 排序列
        /// </summary>
        public OrderByColumn[] Order { get; set; }
    }

    public class OrderByColumn
    {
        private int _column;
        private string _dir;

        /// <summary>
        /// 排序的列对应的索引
        /// </summary>
        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }

        /// <summary>
        /// 排序方向
        /// </summary>
        public string Dir
        {
            get { return _dir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc"; }
            set { _dir = value; }
        }
    }
}