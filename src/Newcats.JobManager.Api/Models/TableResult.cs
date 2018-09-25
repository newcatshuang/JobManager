using System.Collections.Generic;

namespace Newcats.JobManager.Api.Models
{
    /// <summary>
    /// 前端插件datatables.js需要的返回值
    /// </summary>
    public class TableResult
    {
        /// <summary>
        /// 表格内容
        /// </summary>
        public List<object[]> data { get; set; }

        /// <summary>
        /// 本次会话的请求次数
        /// </summary>
        public int draw { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int recordsTotal { get; set; }

        /// <summary>
        /// 筛选记录数
        /// </summary>
        public int recordsFiltered { get; set; }

        /// <summary>
        /// 前端插件datatables.js需要的返回值
        /// </summary>
        /// <param name="data">表格内容</param>
        /// <param name="draw">本次会话的请求次数</param>
        /// <param name="totalCounts">总记录数</param>
        public TableResult(List<object[]> data, int draw, int totalCounts)
        {
            this.data = data;
            this.draw = draw;
            this.recordsTotal = totalCounts;
            this.recordsFiltered = totalCounts;
        }
    }
}