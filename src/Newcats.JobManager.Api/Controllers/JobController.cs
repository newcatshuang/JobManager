using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newcats.JobManager.Api.AppData;
using Newcats.JobManager.Api.Domain.Entity;
using Newcats.JobManager.Api.Domain.IService;
using Newcats.JobManager.Api.Models;
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
        [HttpPost("{id}")]
        [SwaggerResponse(200, type: typeof(BaseResult))]
        public async Task<IActionResult> GetJob(int id)
        {
            JobInfoEntity job = await _jobService.GetJobAsync(id);
            return ToSuccessResult(job);
        }
    }
}