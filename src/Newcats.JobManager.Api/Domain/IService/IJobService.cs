using System.Collections.Generic;
using System.Threading.Tasks;
using Newcats.JobManager.Api.Domain.Entity;
using Newcats.JobManager.Common.DataAccess;
using Newcats.JobManager.Common.DenpendencyInjection;
using Newcats.JobManager.Common.Entity;

namespace Newcats.JobManager.Api.Domain.IService
{
    public interface IJobService : IScopeDependency
    {
        /// <summary>
        /// 插入一条JonInfo记录
        /// </summary>
        /// <param name="jobInfoEntity">JonInfo记录</param>
        /// <returns>是否成功</returns>
        Task<bool> InsertJobInfoAsync(JobInfoEntity jobInfoEntity);

        /// <summary>
        /// 根据主键，更新Job状态
        /// </summary>
        /// <param name="jobId">主键Id</param>
        /// <param name="jobState">状态</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateJobStateAsync(int jobId, JobState jobState);

        /// <summary>
        /// 根据主键，更新一条JonInfo记录
        /// </summary>
        /// <param name="jobId">主键</param>
        /// <param name="dbUpdates">要更新的数据</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateJobAsync(int jobId, IEnumerable<DbUpdate<JobInfoEntity>> dbUpdates);

        /// <summary>
        /// 分页获取JobInfo实体集合
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="dbWheres">筛选条件</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="dbOrderBy">排序条件</param>
        /// <returns>JobInfo实体集合</returns>
        Task<(IEnumerable<JobInfoEntity> list, int totalCount)> GetJobsAsync(int pageIndex, int pageSize, IEnumerable<DbWhere<JobInfoEntity>> dbWheres = null, int? commandTimeout = null, params DbOrderBy<JobInfoEntity>[] dbOrderBy);

        /// <summary>
        /// 根据主键Id,获取一个JobInfo实体
        /// </summary>
        /// <param name="jobId">主键</param>
        /// <returns>JobInfo实体</returns>
        Task<JobInfoEntity> GetJobAsync(int jobId);

        /// <summary>
        /// 获取系统主Job(承载其他Job的系统级Job，唯一)
        /// </summary>
        /// <returns>唯一的系统主Job</returns>
        Task<JobInfoEntity> GetSystemMainJobAsync();

        /// <summary>
        /// 更新一个JobInfo记录
        /// </summary>
        /// <param name="jobInfoEntity">JobInfo记录</param>
        /// <returns>是否成功</returns>
        Task<bool> ModifyJobAsync(JobInfoEntity jobInfoEntity);

        /// <summary>
        /// 获取指定数量的最新的JobLog日志记录
        /// </summary>
        /// <param name="jobId">jobId</param>
        /// <param name="top">需要获取的数量</param>
        /// <returns>JobLog集合</returns>
        Task<IEnumerable<JobLogEntity>> GetLatestJobLogs(int jobId, int top);

        /// <summary>
        /// 分页获取JobLog实体集合
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="dbWheres">筛选条件</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <param name="dbOrderBy">排序条件</param>
        /// <returns>JobLog实体集合</returns>
        Task<(IEnumerable<LogInfoEntity> list, int totalCount)> GetLogsAsync(int pageIndex, int pageSize, IEnumerable<DbWhere<LogInfoEntity>> dbWheres = null, int? commandTimeout = null, params DbOrderBy<LogInfoEntity>[] dbOrderBy);
    }
}