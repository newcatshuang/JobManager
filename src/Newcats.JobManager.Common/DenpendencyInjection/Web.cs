using Microsoft.AspNetCore.Http;

namespace Newcats.JobManager.Common.DenpendencyInjection
{
    /// <summary>
    /// Web操作
    /// </summary>
    public static class Web
    {
        /// <summary>
        /// 初始化Web操作
        /// </summary>
        static Web()
        {
            try
            {
                HttpContextAccessor = Ioc.Create<IHttpContextAccessor>();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Http上下文访问器
        /// </summary>
        public static IHttpContextAccessor HttpContextAccessor { get; set; }

        /// <summary>
        /// 当前Http上下文
        /// </summary>
        public static HttpContext HttpContext => HttpContextAccessor?.HttpContext;
    }
}