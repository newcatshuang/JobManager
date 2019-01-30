using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Newcats.JobManager.Common.NetCore.Util.Helper
{
    public class HttpHelper
    {
        /// <summary>
        /// 获取当前页面客户端的IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIP(HttpContext context, bool tryUseXForwardHeader = true)
        {
            string ip = null;

            // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

            // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
            // for 99% of cases however it has been suggested that a better (although tedious)
            // approach might be to read each IP from right to left and use the first public IP.
            // http://stackoverflow.com/a/43554000/538763
            //
            if (tryUseXForwardHeader)
            {
                ip = GetHeaderValueAs<string>(context, "X-Forwarded-For");//.TrimEnd(',').Split(',').AsEnumerable().Select(s => s.Trim()).ToList().FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(ip))
                    ip = ip.TrimEnd(',').Split(',').AsEnumerable().Select(s => s.Trim()).ToList().FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(ip) && context?.Connection?.RemoteIpAddress != null)
                ip = context.Connection.RemoteIpAddress.ToString();

            if (string.IsNullOrWhiteSpace(ip))
                ip = GetHeaderValueAs<string>(context, "REMOTE_ADDR");

            if (string.IsNullOrWhiteSpace(ip))
                ip = "0.0.0.0";

            return ip;
        }

        private static T GetHeaderValueAs<T>(HttpContext context, string headerName)
        {
            if (context.Request?.Headers?.TryGetValue(headerName, out StringValues values) ?? false)
            {
                string rawValues = values.ToString();   // writes out as Csv when there are multiple.

                if (!string.IsNullOrEmpty(rawValues))
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default(T);
        }
    }
}
