using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newcats.JobManager.Host.NetCore.Manager;
using Topshelf;

namespace Newcats.JobManager.Host.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            IHostBuilder builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IHostLifetime, TopshelfLifetime>();
                    services.AddHostedService<ServiceRunner>();
                });

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