using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newcats.JobManager.Api.AppData;
using Newcats.JobManager.Api.Domain.Entity;
using Newcats.JobManager.Api.Domain.IService;
using Newcats.JobManager.Api.Infrastructure.DataAccess;
using Newcats.JobManager.Api.Infrastructure.Text;
using Newcats.JobManager.Api.Infrastructure.Text.Encrypt;
using Newcats.JobManager.Api.Models;
using Newcats.JobManager.Api.Models.Requests;
using Swashbuckle.AspNetCore.Annotations;

namespace Newcats.JobManager.Api.Controllers
{
    /// <summary>
    /// Job管理控制器
    /// </summary>
    public class JobController : ApiControllerBase
    {
        private readonly IJobService _jobService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="job"></param>
        public JobController(IJobService job)
        {
            _jobService = job;
        }

        /// <summary>
        /// 根据主键Id,获取一个JobInfo实体
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <returns>JobInfo实体</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(200, type: typeof(BaseResult))]
        public async Task<IActionResult> GetJob(int id)
        {
            JobInfoEntity job = await _jobService.GetJobAsync(id);
            return ToSuccessResult(job);
        }

        /// <summary>
        /// 根据搜索条件，分页获取JobInfo实体
        /// </summary>
        /// <param name="request">搜索条件</param>
        /// <returns>JobInfo实体集合</returns>
        [HttpPost]
        [SwaggerResponse(200, type: typeof(TableResult))]
        public async Task<IActionResult> GetJobList([FromForm] JobListRequest request)
        {
            #region 筛选条件
            List<DbWhere<JobInfoEntity>> dbWheres = new List<DbWhere<JobInfoEntity>>();
            if (request.Id.HasValue && request.Id.Value > 0)
                dbWheres.Add(new DbWhere<JobInfoEntity>(j => j.Id, request.Id.Value));
            if (!string.IsNullOrWhiteSpace(request.Name))
                dbWheres.Add(new DbWhere<JobInfoEntity>(j => j.Name, request.Name, OperateType.Like));
            if (request.JobLevel.HasValue)
                dbWheres.Add(new DbWhere<JobInfoEntity>(j => j.JobLevel, request.JobLevel.Value));
            if (!string.IsNullOrWhiteSpace(request.AssemblyName))
                dbWheres.Add(new DbWhere<JobInfoEntity>(j => j.AssemblyName, request.AssemblyName, OperateType.Like));
            if (!string.IsNullOrWhiteSpace(request.ClassName))
                dbWheres.Add(new DbWhere<JobInfoEntity>(j => j.ClassName, request.ClassName, OperateType.Like));
            if (!string.IsNullOrWhiteSpace(request.CronExpression))
                dbWheres.Add(new DbWhere<JobInfoEntity>(j => j.CronExpression, request.CronExpression, OperateType.Like));
            if (request.LastFireTimeStart.HasValue)
                dbWheres.Add(new DbWhere<JobInfoEntity>(j => j.LastFireTime, request.LastFireTimeStart.Value, OperateType.GreaterEqual));
            if (request.LastFireTimeEnd.HasValue)
                dbWheres.Add(new DbWhere<JobInfoEntity>(j => j.LastFireTime, request.LastFireTimeEnd.Value, OperateType.LessEqual));
            if (request.NextFireTimeStart.HasValue)
                dbWheres.Add(new DbWhere<JobInfoEntity>(j => j.NextFireTime, request.NextFireTimeStart.Value, OperateType.GreaterEqual));
            if (request.NextFireTimeEnd.HasValue)
                dbWheres.Add(new DbWhere<JobInfoEntity>(j => j.NextFireTime, request.NextFireTimeEnd.Value, OperateType.LessEqual));
            if (request.State.HasValue)
                dbWheres.Add(new DbWhere<JobInfoEntity>(j => j.State, request.State.Value));
            if (request.Disabled.HasValue)
                dbWheres.Add(new DbWhere<JobInfoEntity>(j => j.Disabled, request.Disabled.Value));
            #endregion

            #region 排序和分页
            string orderByFields = "Id,Name,JobLevel,AssemblyName,CronExpression,LastFireTime,NextFireTime,FireCount,State,Disabled";//表格排序对应的字段
            string orderByColumn = orderByFields.Split(',')[request.Order[0].Column];
            bool isAsc = request.Order[0].Dir.Equals("asc", StringComparison.OrdinalIgnoreCase);
            int pageIndex = request.Start / request.Length;//从第0页开始 
            #endregion

            #region 获取数据
            var (list, totals) = await _jobService.GetJobsAsync(pageIndex, request.Length, dbWheres, null, new DbOrderBy<JobInfoEntity>(orderByColumn, isAsc));
            #endregion

            #region 返回数据
            List<object[]> retTable = new List<object[]>();
            if (list != null && list.Any())
            {
                foreach (JobInfoEntity item in list)
                {
                    #region 填充一行
                    List<object> retRow = new List<object>();
                    retRow.Add(item.Id);
                    retRow.Add(item.Name);
                    switch (item.JobLevel)
                    {
                        case JobLevel.Business:
                            retRow.Add(item.JobLevel.GetDescription().GetSpanHtml("#00c5dc"));
                            break;
                        case JobLevel.Test:
                            retRow.Add(item.JobLevel.GetDescription().GetSpanHtml("#c4c5d6"));
                            break;
                        case JobLevel.System:
                            retRow.Add(item.JobLevel.GetDescription().GetSpanHtml("#FF0000"));
                            break;
                        default:
                            retRow.Add(item.JobLevel.GetDescription());
                            break;
                    }
                    retRow.Add($"{item.AssemblyName}<br/>{item.ClassName}");
                    retRow.Add(item.CronExpression);
                    retRow.Add(item.LastFireTime);
                    retRow.Add(item.NextFireTime);
                    retRow.Add(item.FireCount);
                    switch (item.State)
                    {
                        case JobState.Stopped:
                            retRow.Add(item.State.GetDescription().GetSpanHtml(SpanColor.Metal));
                            break;
                        case JobState.Running:
                            retRow.Add(item.State.GetDescription().GetSpanHtml(SpanColor.Success));
                            break;
                        case JobState.Starting:
                            retRow.Add(item.State.GetDescription().GetSpanHtml(SpanColor.Primary));
                            break;
                        case JobState.Stopping:
                            retRow.Add(item.State.GetDescription().GetSpanHtml(SpanColor.Warning));
                            break;
                        case JobState.Updating:
                            retRow.Add(item.State.GetDescription().GetSpanHtml(SpanColor.Info));
                            break;
                        case JobState.FireNow:
                            retRow.Add(item.State.GetDescription().GetSpanHtml(SpanColor.Focus));
                            break;
                        default:
                            retRow.Add(item.State.GetDescription());
                            break;
                    }
                    if (item.Disabled)
                        retRow.Add("禁用".GetSpanHtml("#a94442"));
                    else
                        retRow.Add("启用".GetSpanHtml("#3c763d"));

                    StringBuilder btnHtml = new StringBuilder();
                    btnHtml.AppendFormat("<a class='btn btn-xs btn-primary' href='javascript:;' onclick='TableAjax.ShowLogTable({0},this)'>日志</a>", item.Id);
                    btnHtml.AppendFormat("<a class='btn btn-xs btn-primary' href='javascript:;' onclick='TableAjax.ShowEditModal({0},this)'>编辑</a>", item.Id);
                    if (item.Disabled)
                        btnHtml.AppendFormat("<a class='btn btn-xs btn-primary' href='javascript:;' onclick='TableAjax.EnableJob({0},this)'>启用</a>", item.Id);
                    else
                        btnHtml.AppendFormat("<a class='btn btn-xs btn-primary' href='javascript:;' onclick='TableAjax.DisableJob({0},this)'></i>禁用</a>", item.Id);

                    if (!item.Disabled)
                    {
                        if (item.State == JobState.Stopped)
                        {
                            btnHtml.AppendFormat("<a class='btn btn-xs btn-primary' href='javascript:;' onclick='TableAjax.StartJob({0},this)'>启动</a>", item.Id);
                        }
                        else if (item.State == JobState.Running)
                        {
                            btnHtml.AppendFormat("<a class='btn btn-xs btn-primary' href='javascript:;' onclick='TableAjax.StopJob({0},this)'>停止</a>", item.Id);
                            btnHtml.AppendFormat("<a class='btn btn-xs btn-primary' href='javascript:;' onclick='TableAjax.FireNowJob({0},this)'>执行一次</a>", item.Id);
                        }
                    }
                    retRow.Add(btnHtml.ToString());
                    retTable.Add(retRow.ToArray());
                    #endregion
                }
            }
            return Json(new TableResult(retTable, request.Draw, totals));
            #endregion
        }

        /// <summary>
        /// 新增一条JobInfo记录
        /// </summary>
        /// <param name="jobInfo">JobInfo实体</param>
        /// <returns>执行结果</returns>
        [HttpPost]
        [SwaggerResponse(200, type: typeof(BaseResult))]
        public async Task<IActionResult> AddJob([FromForm] JobInfoEntity jobInfo)
        {
            JobInfoEntity job = await _jobService.GetJobAsync(jobInfo.Id);
            if (job != null)
                return ToFailResult(-1, $"此Job已经存在！(JobId={jobInfo.Id})");
            if (string.IsNullOrWhiteSpace(jobInfo.Name))
                return ToFailResult(-2, "Job名称不能为空！");
            if (jobInfo.JobLevel != JobLevel.Business && jobInfo.JobLevel != JobLevel.System && jobInfo.JobLevel != JobLevel.Test)
                return ToFailResult(-3, "请选择Job等级！");
            JobInfoEntity mainJob = await _jobService.GetSystemMainJobAsync();
            if (mainJob == null)
                return ToFailResult(-4, "初始配置异常，请联系开发人员！");
            if (jobInfo.AssemblyName.Equals(mainJob.AssemblyName, StringComparison.OrdinalIgnoreCase) || jobInfo.ClassName.Equals(mainJob.ClassName, StringComparison.OrdinalIgnoreCase))
                return ToFailResult(-5, $"特定的程序集名称和类名不允许重复！(程序集名称={jobInfo.AssemblyName})（类名={jobInfo.ClassName}）");
            if (string.IsNullOrWhiteSpace(jobInfo.CronExpression))
                return ToFailResult(-6, "Cron表达式不能为空！");
            if (!Infrastructure.Helper.CronHelper.CronExpression.IsValidExpression(jobInfo.CronExpression))
                return ToFailResult(-7, "Cron表达式非法！");
            bool success = await _jobService.InsertJobInfoAsync(jobInfo);
            if (success)
                return ToSuccessResult();
            else
                return ToFailResult(-8, "插入数据库失败，请刷新重试！");
        }

        /// <summary>
        /// 更新给定的JobInfo记录
        /// </summary>
        /// <param name="jobInfo">JobInfo实体</param>
        /// <returns>执行结果</returns>
        [HttpPost]
        [SwaggerResponse(200, type: typeof(BaseResult))]
        public async Task<IActionResult> ModifyJob([FromForm] JobInfoEntity jobInfo)
        {
            JobInfoEntity job = await _jobService.GetJobAsync(jobInfo.Id);
            if (job == null)
                return ToFailResult(-1, $"此Job不存在！(JobId={jobInfo.Id})");
            if (string.IsNullOrWhiteSpace(jobInfo.Name))
                return ToFailResult(-2, "Job名称不能为空！");
            if (jobInfo.JobLevel != JobLevel.Business && jobInfo.JobLevel != JobLevel.System && jobInfo.JobLevel != JobLevel.Test)
                return ToFailResult(-3, "请选择Job等级！");
            JobInfoEntity mainJob = await _jobService.GetSystemMainJobAsync();
            if (mainJob == null)
                return ToFailResult(-4, "初始配置异常，请联系开发人员！");
            if (jobInfo.AssemblyName.Equals(mainJob.AssemblyName, StringComparison.OrdinalIgnoreCase) || jobInfo.ClassName.Equals(mainJob.ClassName, StringComparison.OrdinalIgnoreCase))
                return ToFailResult(-5, $"特定的程序集名称和类名不允许修改！(程序集名称={jobInfo.AssemblyName})（类名={jobInfo.ClassName}）");
            if (string.IsNullOrWhiteSpace(jobInfo.CronExpression))
                return ToFailResult(-6, "Cron表达式不能为空！");
            if (!Infrastructure.Helper.CronHelper.CronExpression.IsValidExpression(jobInfo.CronExpression))
                return ToFailResult(-7, "Cron表达式非法！");
            bool success = await _jobService.ModifyJobAsync(jobInfo);
            if (success)
                return ToSuccessResult();
            else
                return ToFailResult(-8, "更新数据库失败，请刷新重试！");
        }

        /// <summary>
        /// 根据JobId获取最新的20条日志记录
        /// </summary>
        /// <param name="jobId">主键JobId</param>
        /// <returns>JobLogEntity集合</returns>
        [HttpPost]
        [SwaggerResponse(200, type: typeof(JobLogEntity))]
        public async Task<IActionResult> GetLatestJobLogs([FromForm] int jobId)
        {
            IEnumerable<JobLogEntity> list = await _jobService.GetLatestJobLogs(jobId, 20);
            if (list != null)
                return Json(list.Select(p => new { FireTime = p.FireTime, FireDuration = p.FireDuration, FireState = p.FireState.GetDescription(), Content = p.Content }));
            return Json(list);
        }

        /// <summary>
        /// 根据JobId,启用Job
        /// </summary>
        /// <param name="jobId">JobId</param>
        /// <returns>执行结果</returns>
        [HttpPost]
        [SwaggerResponse(200, type: typeof(BaseResult))]
        public async Task<IActionResult> EnableJob([FromForm] int jobId)
        {
            JobInfoEntity job = await _jobService.GetJobAsync(jobId);
            if (job == null)
                return ToFailResult(-1, $"此Job不存在！(JobId={jobId})");
            if (!job.Disabled)
                return ToFailResult(-2, "只有处于禁用状态的Job才能启用！");
            bool success = await _jobService.UpdateJobAsync(jobId, new List<DbUpdate<JobInfoEntity>> { new DbUpdate<JobInfoEntity>(j => j.Disabled, false) });
            if (success)
                return ToSuccessResult("启用成功！");
            else
                return ToFailResult(-3, "启用失败，请刷新重试！");
        }

        /// <summary>
        /// 根据JobId,禁用Job
        /// </summary>
        /// <param name="jobId">JobId</param>
        /// <returns>执行结果</returns>
        [HttpPost]
        [SwaggerResponse(200, type: typeof(BaseResult))]
        public async Task<IActionResult> DisableJob([FromForm] int jobId)
        {
            JobInfoEntity job = await _jobService.GetJobAsync(jobId);
            if (job == null)
                return ToFailResult(-1, $"此Job不存在！(JobId={jobId})");
            if (job.JobLevel == JobLevel.System)
                return ToFailResult(-2, "系统级Job不允许禁用！");
            if (job.Disabled)
                return ToFailResult(-3, "只有处于启用状态的Job才能禁用！");
            bool success = await _jobService.UpdateJobAsync(jobId, new List<DbUpdate<JobInfoEntity>> { new DbUpdate<JobInfoEntity>(j => j.Disabled, true) });
            if (success)
                return ToSuccessResult("禁用成功！");
            else
                return ToFailResult(-4, "禁用失败，请刷新重试！");
        }

        /// <summary>
        /// 根据JobId,启动Job
        /// </summary>
        /// <param name="jobId">JobId</param>
        /// <returns>执行结果</returns>
        [HttpPost]
        [SwaggerResponse(200, type: typeof(BaseResult))]
        public async Task<IActionResult> StartJob([FromForm] int jobId)
        {
            JobInfoEntity job = await _jobService.GetJobAsync(jobId);
            if (job == null)
                return ToFailResult(-1, $"此Job不存在！(JobId={jobId})");
            if (job.Disabled || job.State != JobState.Stopped)
                return ToFailResult(-2, "只有处于启用并且停止状态的Job才能启动！");
            bool success = await _jobService.UpdateJobStateAsync(jobId, JobState.Starting);
            if (success)
                return ToSuccessResult("启动成功，请等待生效！");
            else
                return ToFailResult(-3, "启动失败，请刷新重试！");
        }

        /// <summary>
        /// 根据JobId,停止Job
        /// </summary>
        /// <param name="jobId">JobId</param>
        /// <returns>执行结果</returns>
        [HttpPost]
        [SwaggerResponse(200, type: typeof(BaseResult))]
        public async Task<IActionResult> StopJob([FromForm] int jobId)
        {
            JobInfoEntity job = await _jobService.GetJobAsync(jobId);
            if (job == null)
                return ToFailResult(-1, $"此Job不存在！(JobId={jobId})");
            if (job.JobLevel == JobLevel.System)
                return ToFailResult(-2, "系统级Job不允许停止！");
            if (job.Disabled || job.State != JobState.Running)
                return ToFailResult(-3, "只有处于启用并且运行中状态的Job才能停止！");
            bool success = await _jobService.UpdateJobStateAsync(jobId, JobState.Stopping);
            if (success)
                return ToSuccessResult("停止成功，请等待生效！");
            else
                return ToFailResult(-4, "停止失败，请刷新重试！");
        }

        /// <summary>
        /// 根据JobId,立即执行一次此Job
        /// </summary>
        /// <param name="jobId">JobId</param>
        /// <returns>执行结果</returns>
        [HttpPost]
        [SwaggerResponse(200, type: typeof(BaseResult))]
        public async Task<IActionResult> FireNowJob([FromForm] int jobId)
        {
            JobInfoEntity job = await _jobService.GetJobAsync(jobId);
            if (job == null)
                return ToFailResult(-1, $"此Job不存在！(JobId={jobId})");
            if (job.Disabled || job.State != JobState.Running)
                return ToFailResult(-2, "只有处于启用并且运行中状态的Job才能立即执行一次！");
            bool success = await _jobService.UpdateJobStateAsync(jobId, JobState.FireNow);
            if (success)
                return ToSuccessResult("操作成功，请等待生效！");
            else
                return ToFailResult(-3, "操作失败，请刷新重试！");
        }

        /// <summary>
        /// 上传DLL文件，并保存在JobHost文件夹
        /// </summary>
        /// <param name="dllFile">文件</param>
        /// <returns>执行结果</returns>
        [HttpPost]
        [SwaggerResponse(200, type: typeof(BaseResult))]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile dllFile)
        {
            IFormFile file = HttpContext.Request.Form.Files[0];//方法的参数里面取到的dllFile=null，这里可以取到
            DirectoryInfo baseDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());//当前执行路径
            string hostPath = Path.Combine(baseDirectory.Parent.FullName, "JobHost");//JobHost文件夹的路径
            if (!Directory.Exists(hostPath))
                Directory.CreateDirectory(hostPath);
            string savedFileName = Path.Combine(hostPath, file.FileName);//保存在JobHost文件夹里的文件名
            try
            {
                if (System.IO.File.Exists(savedFileName))
                    System.IO.File.Delete(savedFileName);
                using (FileStream fs = new FileStream(savedFileName, FileMode.CreateNew))
                {
                    await file.CopyToAsync(fs);
                }
                return ToSuccessResult("保存成功！");
            }
            catch (Exception ex)
            {
                return ToFailResult($"保存失败，异常信息：{ex.Message}");
            }
        }

        /// <summary>
        /// 根据搜索条件，分页获取JobLog实体
        /// </summary>
        /// <param name="request">搜索条件</param>
        /// <returns>JobLog实体集合</returns>
        [HttpPost]
        [SwaggerResponse(200, type: typeof(TableResult))]
        public async Task<IActionResult> GetLogList([FromForm] LogListRequest request)
        {
            #region 筛选条件
            List<DbWhere<LogInfoEntity>> dbWheres = new List<DbWhere<LogInfoEntity>>();
            if (request.Id.HasValue && request.Id.Value > 0)
                dbWheres.Add(new DbWhere<LogInfoEntity>(j => j.Id, request.Id.Value));
            if (request.JobId.HasValue && request.JobId.Value > 0)
                dbWheres.Add(new DbWhere<LogInfoEntity>(j => j.JobId, request.JobId.Value));
            if (!string.IsNullOrWhiteSpace(request.JobName))
                dbWheres.Add(new DbWhere<LogInfoEntity>(j => j.JobName, request.JobName));
            if (request.FireTimeStart.HasValue)
                dbWheres.Add(new DbWhere<LogInfoEntity>(j => j.FireTime, request.FireTimeStart.Value, OperateType.GreaterEqual));
            if (request.FireTimeEnd.HasValue)
                dbWheres.Add(new DbWhere<LogInfoEntity>(j => j.FireTime, request.FireTimeEnd, OperateType.LessEqual));
            if (request.FireState.HasValue)
                dbWheres.Add(new DbWhere<LogInfoEntity>(j => j.FireState, request.FireState));
            if (request.CreateTimeStart.HasValue)
                dbWheres.Add(new DbWhere<LogInfoEntity>(j => j.CreateTime, request.CreateTimeStart, OperateType.GreaterEqual));
            if (request.CreateTimeEnd.HasValue)
                dbWheres.Add(new DbWhere<LogInfoEntity>(j => j.CreateTime, request.CreateTimeEnd, OperateType.LessEqual));
            #endregion

            #region 排序和分页
            string orderByFields = "a.Id,a.JobId,b.Name,a.FireTime,a.FireDuration,a.FireState,a.Content,a.CreateTime";//表格排序对应的字段
            string orderByColumn = orderByFields.Split(',')[request.Order[0].Column];
            bool isAsc = request.Order[0].Dir.Equals("asc", StringComparison.OrdinalIgnoreCase);
            int pageIndex = request.Start / request.Length;//从第0页开始 
            #endregion

            #region 获取数据
            var (list, totals) = await _jobService.GetLogsAsync(pageIndex, request.Length, dbWheres, null, new DbOrderBy<LogInfoEntity>(orderByColumn, isAsc));
            #endregion

            #region 返回数据
            List<object[]> retTable = new List<object[]>();
            if (list != null && list.Any())
            {
                foreach (LogInfoEntity item in list)
                {
                    #region 填充一行
                    List<object> retRow = new List<object>();
                    retRow.Add(item.Id);
                    retRow.Add(item.JobId);
                    retRow.Add(item.JobName);
                    retRow.Add(item.FireTime);
                    retRow.Add(item.FireDuration);
                    switch (item.FireState)
                    {
                        case FireState.Success:
                            retRow.Add(item.FireState.GetDescription().GetSpanHtml(SpanColor.Success));
                            break;
                        case FireState.Failed:
                            retRow.Add(item.FireState.GetDescription().GetSpanHtml(SpanColor.Warning));
                            break;
                        case FireState.Error:
                            retRow.Add(item.FireState.GetDescription().GetSpanHtml(SpanColor.Danger));
                            break;
                        default:
                            retRow.Add(item.FireState.GetDescription());
                            break;
                    }
                    retRow.Add(item.Content);
                    retRow.Add(item.CreateTime);
                    retRow.Add(string.Empty);
                    retTable.Add(retRow.ToArray());
                    #endregion
                }
            }
            return Json(new TableResult(retTable, request.Draw, totals));
            #endregion
        }

        /// <summary>
        /// 根据搜索条件，获取JobHost目录下的所有文件
        /// </summary>
        /// <param name="request">搜索条件</param>
        /// <returns>文件信息集合</returns>
        [HttpPost]
        [SwaggerResponse(200, type: typeof(TableResult))]
        public IActionResult GetFileInfoList([FromForm] FileListRequest request)
        {
            #region 获取数据
            FileInfo[] list = GetHostDirectoryInfo().GetFiles();
            #endregion

            #region 搜索、排序、分页
            if (list != null && list.Any())
            {
                if (!string.IsNullOrWhiteSpace(request.Name))
                    list = list.Where(f => f.Name.Contains(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)).ToArray();
                if (request.CreateTimeStart.HasValue)
                    list = list.Where(f => f.CreationTime >= request.CreateTimeStart).ToArray();
                if (request.CreateTimeEnd.HasValue)
                    list = list.Where(f => f.CreationTime <= request.CreateTimeEnd).ToArray();
                if (request.AccessTimeStart.HasValue)
                    list = list.Where(f => f.LastAccessTime >= request.AccessTimeStart).ToArray();
                if (request.AccessTimeEnd.HasValue)
                    list = list.Where(f => f.LastAccessTime <= request.AccessTimeEnd).ToArray();
                if (request.WriteTimeStart.HasValue)
                    list = list.Where(f => f.LastWriteTime >= request.WriteTimeStart).ToArray();
                if (request.WriteTimeEnd.HasValue)
                    list = list.Where(f => f.LastWriteTime <= request.WriteTimeEnd).ToArray();

                int orderIndex = request.Order[0].Column;
                bool isAsc = request.Order[0].Dir.Equals("asc", StringComparison.OrdinalIgnoreCase);
                switch (orderIndex)
                {
                    case 1://Name
                        if (isAsc) list = list.OrderBy(f => f.Name).ToArray();
                        else list = list.OrderByDescending(f => f.Name).ToArray();
                        break;
                    case 2://Length
                        if (isAsc) list = list.OrderBy(f => f.Length).ToArray();
                        else list = list.OrderByDescending(f => f.Length).ToArray();
                        break;
                    case 3://CreationTime
                        if (isAsc) list = list.OrderBy(f => f.CreationTime).ToArray();
                        else list = list.OrderByDescending(f => f.CreationTime).ToArray();
                        break;
                    case 4://LastAccessTime
                        if (isAsc) list = list.OrderBy(f => f.LastAccessTime).ToArray();
                        else list = list.OrderByDescending(f => f.LastAccessTime).ToArray();
                        break;
                    case 5://LastWriteTime
                        if (isAsc) list = list.OrderBy(f => f.LastWriteTime).ToArray();
                        else list = list.OrderByDescending(f => f.LastWriteTime).ToArray();
                        break;
                    default:
                        break;
                }

                int pageIndex = request.Start / request.Length;//从第0页开始 
                list = list.Skip(pageIndex * request.Length).Take(request.Length).ToArray();
            }
            #endregion

            #region 返回数据
            List<object[]> retTable = new List<object[]>();
            if (list != null && list.Any())
            {
                int index = 1;
                foreach (FileInfo item in list)
                {
                    #region 填充一行
                    List<object> retRow = new List<object>();
                    retRow.Add(index);
                    retRow.Add(item.Name);
                    retRow.Add(item.Length);//字节
                    retRow.Add(item.CreationTime);//创建时间
                    retRow.Add(item.LastAccessTime);//访问时间
                    retRow.Add(item.LastWriteTime);//写入时间

                    StringBuilder btnHtml = new StringBuilder();
                    btnHtml.AppendFormat("<a class='btn btn-xs btn-primary' traget='_blank' href='http://localhost:5000/api/Job/Download/{0}'>下载</a>", Encrypt.MD5By32(item.FullName));
                    retRow.Add(btnHtml.ToString());
                    retTable.Add(retRow.ToArray());
                    #endregion
                    index++;
                }
            }
            return Json(new TableResult(retTable, request.Draw, list == null ? 0 : list.Length));
            #endregion
        }

        /// <summary>
        /// 下载指定文件
        /// </summary>
        /// <param name="fileName">文件全路径MD5</param>
        /// <returns>文件流</returns>
        [HttpGet("{fileName}")]
        [SwaggerResponse(200, type: typeof(FileResult))]
        public IActionResult Download(string fileName)
        {
            FileInfo[] list = GetHostDirectoryInfo().GetFiles();

            FileInfo file = list.Where(f => Encrypt.MD5By32(f.FullName).Equals(fileName)).FirstOrDefault();
            FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.ReadWrite);
            return File(fs, "application/octet-stream", file.Name, false);
        }

        /// <summary>
        /// 获取JobHost的根目录
        /// </summary>
        /// <returns>JobHost目录</returns>
        private DirectoryInfo GetHostDirectoryInfo()
        {
            DirectoryInfo baseDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());//当前执行路径 new DirectoryInfo("D:\\JobManager\\JobApi");
            string hostPath = Path.Combine(baseDirectory.Parent.FullName, "JobHost");//JobHost文件夹的路径
            if (!Directory.Exists(hostPath))
                Directory.CreateDirectory(hostPath);
            return new DirectoryInfo(hostPath);
        }
    }
}