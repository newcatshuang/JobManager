using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newcats.JobManager.Common.NetCore.DenpendencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Newcats.JobManager.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                {
                    //忽略循环引用
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                    //不使用驼峰样式的key
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();

                    //设置时间格式
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                });

            //添加跨域
            services.AddCors(opt => opt.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .WithOrigins("http://localhost:10001")
                       .AllowCredentials();
            }));

            //注册Swagger生成器，定义一个和多个Swagger文档
            //注意：使用Swagger，必须为每个Controller的Action方法显示指定Route/HttpVerb
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "JobManager Api",
                    Description = "The Job Manager Api document",
                    Contact = new Contact
                    {
                        Name = "Newcats",
                        Email = string.Empty,
                        Url = "https://www.newcats.xyz"
                    },
                    License = new License
                    {
                        Name = "MIT License",
                        Url = "https://www.newcats.xyz/license.html"
                    }
                });

                //Set the comments path for the Swagger JSON and UI
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            });

            services.AddHttpContextAccessor();

            //注册Autofac依赖注入服务
            return services.AddAutofac();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //启用跨域
            app.UseCors("CorsPolicy");

            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();

            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Job Manager Api v1");
                c.RoutePrefix = "doc";//http://localhost:43655/doc/index.html
            });

            app.UseMvc();
        }
    }
}