using Microsoft.AspNetCore.Mvc;
using Newcats.JobManager.Api.Models;

namespace Newcats.JobManager.Api.AppData
{
    /// <summary>
    /// Api控制器基类
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApiControllerBase : Controller
    {
        #region 返回结果
        /// <summary>
        /// 返回成功Json结果(默认数据为:res.Code=0,res.Message="success",res.Data=null)
        /// </summary>
        /// <returns>Json结果</returns>
        protected JsonResult ToSuccessResult()
        {
            return ToBaseResult(0, "success", null);
        }

        /// <summary>
        /// 返回成功Json结果(默认数据为:res.Code=0,res.Message=message,res.Data=null)
        /// </summary>
        /// <param name="message">提示消息</param>
        /// <returns>Json结果</returns>
        protected JsonResult ToSuccessResult(string message)
        {
            return ToBaseResult(0, message, null);
        }

        /// <summary>
        /// 返回成功Json结果(默认数据为:res.Code=0,res.Message="success",res.Data=data)
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>Json结果</returns>
        protected JsonResult ToSuccessResult(object data)
        {
            return ToBaseResult(0, "success", data);
        }

        /// <summary>
        /// 返回成功Json结果(默认数据为:res.Code=0,res.Message=message,res.Data=data)
        /// </summary>
        /// <param name="message">提示消息</param>
        /// <param name="data">数据</param>
        /// <returns>Json结果</returns>
        protected JsonResult ToSuccessResult(string message, object data)
        {
            return ToBaseResult(0, message, data);
        }

        /// <summary>
        /// 返回失败Json结果(默认数据为:res.Code=-1,res.Message="failed",res.Data=null)
        /// </summary>
        /// <returns>Json结果</returns>
        protected JsonResult ToFailResult()
        {
            return ToBaseResult(-1, "failed", null);
        }

        /// <summary>
        /// 返回失败Json结果(默认数据为:res.Code=-1,res.Message=message,res.Data=null)
        /// </summary>
        /// <param name="message">提示消息</param>
        /// <returns>Json结果</returns>
        protected JsonResult ToFailResult(string message)
        {
            return ToBaseResult(-1, message, null);
        }

        /// <summary>
        /// 返回失败Json结果(默认数据为:res.Code=code,res.Message=message,res.Data=null)
        /// </summary>
        /// <param name="code">代码，一般code=0表示成功</param>
        /// <param name="message">提示消息</param>
        /// <returns>Json结果</returns>
        protected JsonResult ToFailResult(int code, string message)
        {
            return ToBaseResult(code, message, null);
        }

        /// <summary>
        /// 返回Json结果基本返回类型,结果分别为:res.Code,res.Message,res.Data
        /// </summary>
        /// <param name="code">代码，一般code=0表示成功</param>
        /// <param name="message">提示消息</param>
        /// <param name="data">数据</param>
        /// <returns>Json结果</returns>
        protected JsonResult ToBaseResult(int code, string message, object data)
        {
            return Json(new BaseResult(code, message, data));
        }
        #endregion
    }
}