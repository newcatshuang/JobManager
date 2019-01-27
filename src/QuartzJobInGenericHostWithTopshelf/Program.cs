using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Topshelf;

namespace QuartzJobInGenericHostWithTopshelf
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IHostLifetime, TopshelfLifetime>();
                    services.AddHostedService<QuartzJobService>();
                });

            //builder.Build().Run();

            HostFactory.Run(x =>
            {
                x.SetServiceName("QuartzJobInGenericHostWithTopshelf");
                x.SetDisplayName("QuartzJobInGenericHostWithTopshelf");
                x.SetDescription("QuartzJobInGenericHostWithTopshelf");

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