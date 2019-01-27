using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;

namespace QuartzJobInGenericHostWithTopshelf
{
    public class QuartzJobService : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            NameValueCollection props = new NameValueCollection()
            {
                { "quartz.jobStore.type" ,"Quartz.Simpl.RAMJobStore,Quartz" },//默认使用内存存储
                //{ "quartz.jobStore.type","Quartz.Impl.AdoJobStore.JobStoreTX,Quartz"},//使用ado存储
                //{ "quartz.jobStore.driverDelegateType","Quartz.Impl.AdoJobStore.StdAdoDelegate,Quartz" },//默认ado委托
                //{ "quartz.jobStore.driverDelegateType","Quartz.Impl.AdoJobStore.SqlServerDelegate,Quartz" },//sql server ado委托//性能更好
                //{ "quartz.jobStore.tablePrefix","QRTZ_"},//默认表前缀配置
                //{ "quartz.jobStore.dataSource","NewcatsQuartzData" },//ado数据源名称配置
                //{ "quartz.dataSource.NewcatsQuartzData.connectionString","Data Source = .; Initial Catalog =NewcatsDB20170627; User ID = sa; Password = 123456;" },//链接字符串

                //{ "quartz.dataSource.NewcatsQuartzData.provider","SqlServer" },//ado 驱动
                //目前支持一下驱动
                //SqlServer - .NET Framework 2.0的SQL Server驱动程序
                //OracleODP - Oracle的Oracle驱动程序
                //OracleODPManaged - Oracle的Oracle 11托管驱动程序
                //MySql - MySQL Connector / .NET
                //SQLite - SQLite ADO.NET Provider
                //SQLite-Microsoft - Microsoft SQLite ADO.NET Provider
                //Firebird - Firebird ADO.NET提供程序
                //Npgsql - PostgreSQL Npgsql

                //{"quartz.jobStore.useProperties","true" },//配置AdoJobStore以将字符串用作JobDataMap值（推荐）
                //{"quartz.serializer.type","json" }//ado 序列化策略//可选值为json/binary（推荐json）
            };

            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            IScheduler scheduler = await factory.GetScheduler();//获取调度器

            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<WriteLogJob>()
                .StoreDurably()
                .RequestRecovery()
                .WithIdentity("job1", "group1")
                .Build();
            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .StartNow()
                .WithIdentity("t1", "group1")
                .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(1)
                .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
