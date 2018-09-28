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
        public async Task<IActionResult> GetJobList(JobListRequest request)
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
            int pageIndex = (request.Start / request.Length);//从第0页开始 
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
                    retRow.Add(item.JobLevel.GetDescription());
                    retRow.Add($"{item.AssemblyName}<br/>{item.ClassName}");
                    retRow.Add(item.CronExpression);
                    retRow.Add(item.LastFireTime);
                    retRow.Add(item.NextFireTime);
                    retRow.Add(item.FireCount);
                    retRow.Add(item.State.GetDescription());
                    if (item.Disabled)
                        retRow.Add("<span class='label label-sm label-danger'>禁用</span>");
                    else
                        retRow.Add("<span class='label label-sm label-success'>启用</span>");

                    StringBuilder btnHtml = new StringBuilder();
                    btnHtml.AppendFormat("<a class='btn btn-xs btn-primary' href='javascript:;' onclick='TableAjax.ShowEditModal({0},this)'><i class='fa fa-edit'></i>编辑</a>", item.Id);
                    if (item.Disabled)
                        btnHtml.AppendFormat("<a class='btn default btn-xs green' href='javascript:;' onclick='TableAjax.EnableAdminUser({0},this)'><i class='fa fa-smile-o'></i>启用</a>", item.Id);
                    else
                        btnHtml.AppendFormat("<a class='btn default btn-xs red' href='javascript:;' onclick='TableAjax.DisableAdminUser({0},this)'><i class='fa fa-ban'></i>禁用</a>", item.Id);
                    btnHtml.AppendFormat("<a class='btn default btn-xs purple' href='javascript:;' onclick='TableAjax.ShowRoleModal({0},this)'><i class='fa fa-lock'></i>分配角色</a>", item.Id);
                    retRow.Add(btnHtml.ToString());
                    retTable.Add(retRow.ToArray());
                    #endregion
                }
            }
            return Json(new TableResult(retTable, request.Draw, totals));
            #endregion
        }
    }
}