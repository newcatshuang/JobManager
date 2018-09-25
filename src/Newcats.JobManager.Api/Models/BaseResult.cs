namespace Newcats.JobManager.Api.Models
{
    /// <summary>
    /// Json结果基本返回类型,结果分别为:res.Code,res.Message,res.Data
    /// </summary>
    public class BaseResult
    {
        /// <summary>
        /// 代码，一般code=0表示成功
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 提示消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Json结果基本返回类型,结果分别为:res.Code,res.Message,res.Data
        /// </summary>
        /// <param name="code">代码，一般code=0表示成功</param>
        /// <param name="message">提示消息</param>
        public BaseResult(int code, string message)
        {
            this.Code = code;
            this.Message = message;
            this.Data = null;
        }

        /// <summary>
        /// Json结果基本返回类型,结果分别为:res.Code,res.Message,res.Data
        /// </summary>
        /// <param name="code">代码，一般code=0表示成功</param>
        /// <param name="message">提示消息</param>
        /// <param name="data">数据</param>
        public BaseResult(int code, string message, object data)
        {
            this.Code = code;
            this.Message = message;
            this.Data = data;
        }
    }
}