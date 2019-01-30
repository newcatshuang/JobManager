using System.ComponentModel;

namespace Newcats.JobManager.Api.Infrastructure.Text
{
    public enum SpanColor
    {
        /// <summary>
        /// Primary(#5867dd)
        /// </summary>
        [Description("#5867dd")]
        Primary = 0,

        /// <summary>
        /// Success(#34bfa3)
        /// </summary>
        [Description("#34bfa3")]
        Success = 1,

        /// <summary>
        /// Warning(#ffb822)
        /// </summary>
        [Description("#ffb822")]
        Warning = 2,

        /// <summary>
        /// Danger(#f4516c)
        /// </summary>
        [Description("#f4516c")]
        Danger = 3,

        /// <summary>
        /// Metal(#c4c5d6)
        /// </summary>
        [Description("#c4c5d6")]
        Metal = 4,

        /// <summary>
        /// Brand(#716aca)
        /// </summary>
        [Description("#716aca")]
        Brand = 5,

        /// <summary>
        /// Info(#36a3f7)
        /// </summary>
        [Description("#36a3f7")]
        Info = 6,

        /// <summary>
        /// Focus(#9816f4)
        /// </summary>
        [Description("#9816f4")]
        Focus = 7
    }

    public static class StringExtensions
    {
        /// <summary>
        /// 根据枚举颜色，生成字符串的Span类型html标签
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="color">颜色枚举</param>
        /// <returns>生成的span标签</returns>
        public static string GetSpanHtml(this string str, SpanColor color)
        {
            switch (color)
            {
                case SpanColor.Primary:
                    str = $"<span class='label label-sm' style='background-color:#5867dd'>{str}</span>";
                    break;
                case SpanColor.Success:
                    str = $"<span class='label label-sm' style='background-color:#34bfa3'>{str}</span>";
                    break;
                case SpanColor.Warning:
                    str = $"<span class='label label-sm' style='background-color:#ffb822'>{str}</span>";
                    break;
                case SpanColor.Danger:
                    str = $"<span class='label label-sm' style='background-color:#f4516c'>{str}</span>";
                    break;
                case SpanColor.Metal:
                    str = $"<span class='label label-sm' style='background-color:#c4c5d6'>{str}</span>";
                    break;
                case SpanColor.Brand:
                    str = $"<span class='label label-sm' style='background-color:#716aca'>{str}</span>";
                    break;
                case SpanColor.Info:
                    str = $"<span class='label label-sm' style='background-color:#36a3f7'>{str}</span>";
                    break;
                case SpanColor.Focus:
                    str = $"<span class='label label-sm' style='background-color:#9816f4'>{str}</span>";
                    break;
                default:
                    break;
            }
            return str;
        }

        /// <summary>
        /// 根据给定的16进制颜色，生成字符串的Span类型html标签
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="colorString">16进制颜色（#00c5dc）</param>
        /// <returns>生成的span标签</returns>
        public static string GetSpanHtml(this string str, string colorString)
        {
            return $"<span class='label label-sm' style='background-color:{colorString}'>{str}</span>";
        }
    }
}