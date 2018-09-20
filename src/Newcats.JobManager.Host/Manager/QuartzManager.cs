using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newcats.JobManager.Host.Domain.Entity;
using Newcats.JobManager.Host.Domain.Service;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;

namespace Newcats.JobManager.Host.Manager
{
    public class QuartzManager
    {
        /// <summary>
        /// 从程序集中加载指定类
        /// </summary>
        /// <param name="assemblyName">含后缀的程序集名</param>
        /// <param name="className">含命名空间完整类名</param>
        /// <returns></returns>
        private Type GetClassInfo(string assemblyName, string className)
        {
            Type type = null;
            try
            {
                string file = GetAbsolutePath(Path.Combine("JobItems", assemblyName));
                if (!File.Exists(file))
                {
                    file = GetAbsolutePath(assemblyName);
                }
                Assembly assembly = Assembly.LoadFrom(file);
                type = assembly.GetType(className, true, true);
            }
            catch
            {
            }
            return type;
        }

        /// <summary>
        ///  获取文件的绝对路径
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns></returns>
        public string GetAbsolutePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                throw new ArgumentNullException(nameof(relativePath));
            }
            relativePath = relativePath.Replace("/", "\\");
            if (relativePath[0] == '\\')
            {
                relativePath = relativePath.Remove(0, 1);
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        }


        /// <summary>
        /// Job调度
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="jobInfo"></param>
        public void ScheduleJob(IScheduler scheduler, JobInfoEntity jobInfo)
        {
            if (CronExpression.IsValidExpression(jobInfo.CronExpression))
            {
                Type type = GetClassInfo(jobInfo.AssemblyName, jobInfo.ClassName);
                if (type != null)
                {
                    IJobDetail job = new JobDetailImpl(jobInfo.Id.ToString(), jobInfo.Id.ToString() + "Group", type);
                    job.JobDataMap.Add("Parameters", jobInfo.JobArgs);
                    job.JobDataMap.Add("JobName", jobInfo.Name);

                    CronTriggerImpl trigger = new CronTriggerImpl();
                    trigger.CronExpressionString = jobInfo.CronExpression;
                    trigger.Name = jobInfo.Id.ToString();
                    trigger.Description = jobInfo.Description;
                    trigger.StartTimeUtc = DateTime.UtcNow;
                    trigger.Group = jobInfo.Id + "TriggerGroup";
                    scheduler.ScheduleJob(job, trigger);
                }
                else
                {
                    //new JobService().WriteBackgroundJoLog(jobInfo.Id, jobInfo.Name, DateTime.Now, jobInfo.AssemblyName + jobInfo.ClassName + "无效，无法启动该任务");
                    JobService.AddLog(new JobLogEntity
                    {
                        JobId = jobInfo.Id,
                        ExecutionTime = DateTime.Now,
                        RunLog = $"[{jobInfo.AssemblyName}]/[{jobInfo.ClassName}]无效，无法启动该任务！"
                    });
                }
            }
            else
            {
                //new JobService().WriteBackgroundJoLog(jobInfo.Id, jobInfo.Name, DateTime.Now, jobInfo.CronExpression + "不是正确的Cron表达式,无法启动该任务");
                JobService.AddLog(new JobLogEntity
                {
                    JobId = jobInfo.Id,
                    ExecutionTime = DateTime.Now,
                    RunLog = $"[{jobInfo.CronExpression}]不是正确的Cron表达式,无法启动该任务！"
                });
            }
        }


        /// <summary>
        /// Job状态管控
        /// </summary>
        /// <param name="Scheduler"></param>
        public async void JobScheduler(IScheduler Scheduler)
        {
            IEnumerable<JobInfoEntity> list = JobService.GetAllowScheduleJobs();
            if (list != null && list.Any())
            {
                foreach (JobInfoEntity jobInfo in list)
                {
                    JobKey jobKey = new JobKey(jobInfo.Id.ToString(), jobInfo.Id.ToString() + "Group");
                    if (await Scheduler.CheckExists(jobKey) == false)
                    {
                        if (jobInfo.State == JobState.Running || jobInfo.State == JobState.Starting)
                        {
                            ScheduleJob(Scheduler, jobInfo);
                            if (await Scheduler.CheckExists(jobKey) == false)
                            {
                                JobService.UpdateJobState(jobInfo.Id, JobState.Stop);
                            }
                            else
                            {
                                JobService.UpdateJobState(jobInfo.Id, JobState.Running);
                            }
                        }
                        else if (jobInfo.State == JobState.Stopping)
                        {
                            JobService.UpdateJobState(jobInfo.Id, JobState.Stop);
                        }
                    }
                    else
                    {
                        if (jobInfo.State == JobState.Stopping)
                        {
                            await Scheduler.DeleteJob(jobKey);
                            JobService.UpdateJobState(jobInfo.Id, JobState.Stop);
                        }
                        else if (jobInfo.State == JobState.Starting)
                        {
                            JobService.UpdateJobState(jobInfo.Id, JobState.Running);
                        }
                    }
                }
            }
        }

    }
}
