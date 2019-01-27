using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newcats.JobManager.Common.NetCore.DataAccess;
using Newcats.JobManager.Common.NetCore.Entity;
using Newcats.JobManager.Host.NetCore.Logger;
using Newcats.JobManager.Host.NetCore.Manager;
using Newcats.JobManager.Host.NetCore.Service;
using Quartz;
using Topshelf;

namespace Newcats.JobManager.Host.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            IHostBuilder builder = new HostBuilder()
                .ConfigureHostConfiguration(cfg =>
                {
                    cfg.SetBasePath(Directory.GetCurrentDirectory());
                    cfg.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.AddFilter("System", LogLevel.Warning);
                    logging.AddFilter("Microsoft", LogLevel.Warning);
                    logging.AddLog4Net();//自动记录全局的异常日志（不需要自己写全局异常过滤器记录）
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IHostLifetime, TopshelfLifetime>();
                    services.AddScoped<IRepository<JobInfoEntity, int>, Repository<JobInfoEntity, int>>();
                    services.AddScoped<IRepository<JobLogEntity, long>, Repository<JobLogEntity, long>>();
                    services.AddHostedService<ServiceRunner>();
                    services.AddSingleton<IQuartzManager, QuartzManager>();
                    services.AddScoped<IJobService, JobService>();
                    services.AddSingleton<IJobListener, JobListener>();
                });

            //builder.Build().Run();

            HostFactory.Run(x =>
            {
                x.RunAsLocalSystem();

                x.SetDescription("作业调度管理器的托管服务");
                x.SetDisplayName("作业调度服务");
                x.SetServiceName("JobManagerHostServer");

                x.Service<IHost>(s =>
                {
                    s.ConstructUsing(() => builder.Build());
                    s.WhenStarted(service =>
                    {
                        service.StartAsync();
                    });
                    s.WhenStopped(service =>
                    {
                        service.StopAsync();
                    });
                });
            });
        }
    }
}