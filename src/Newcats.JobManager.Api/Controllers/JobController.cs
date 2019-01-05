using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newcats.JobManager.Api.AppData;
using Newcats.JobManager.Api.Domain.Entity;
using Newcats.JobManager.Api.Domain.IService;
using Newcats.JobManager.Api.Infrastructure.DataAccess;
using Newcats.JobManager.Api.Infrastructure.Text;
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
    }
}