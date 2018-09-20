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
                x.SetDescription("JobManagerHostServer");
                x.SetDisplayName("JobManagerHostServer");
                x.SetServiceName("JobManagerHostServer");
            });
        }
    }
}