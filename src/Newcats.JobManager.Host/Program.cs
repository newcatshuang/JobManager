using System;
using System.IO;
using log4net.Config;
using Newcats.JobManager.Host.Manager;
using Topshelf;

namespace Newcats.JobManager.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            FileInfo logConfig = new FileInfo($"{AppDomain.CurrentDomain.BaseDirectory}log4net.config");
            XmlConfigurator.ConfigureAndWatch(logConfig);

            HostFactory.Run(x =>
            {
                x.UseLog4Net();
                x.RunAsLocalSystem();
                x.Service<ServiceRunner>();
                x.SetDescription("作业调度管理器的托管服务");
                x.SetDisplayName("作业调度服务");
                x.SetServiceName("JobManagerHostServer");
            });
        }
    }
}