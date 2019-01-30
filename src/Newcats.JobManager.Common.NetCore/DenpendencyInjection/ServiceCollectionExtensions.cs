using System;
using Microsoft.Extensions.DependencyInjection;

namespace Newcats.JobManager.Api.Infrastructure.DenpendencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Autofac依赖注入服务,默认注册Dapper仓储
        /// </summary>
        public static IServiceProvider AddAutofac(this IServiceCollection services)
        {
            return new DependencyConfiguration(services).Config();
        }
    }
}