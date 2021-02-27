using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Swagger;

namespace Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 日志配置
            LogConfig();

            #region 跨域     
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSameDomain", policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyMethod()
                        .AllowAnyHeader();

                    var allowedHosts = Configuration.GetSection("AllowedHosts").Get<List<string>>();
                    if (allowedHosts?.Any(t => t == "*") ?? false)
                        policyBuilder.AllowAnyOrigin(); //允许任何来源的主机访问
                    else if (allowedHosts?.Any() ?? false)
                        policyBuilder.AllowCredentials().WithOrigins(allowedHosts.ToArray()); //允许类似http://localhost:8080等主机访问
                });
            });
            #endregion

            services.AddHttpClient(string.Empty, (sp, http) =>
            {
                http.Timeout = new TimeSpan(0, 2, 0);
            })
            .ConfigurePrimaryHttpMessageHandler(() => 
            {
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
                };
            });

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(@"DataProtection-Keys"));
            
            services.AddControllersWithViews(t =>
            {
                //t.Filters.Add<AuthorizationFilter>();
            })
            .AddNewtonsoftJson();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "MsSystem API"
                });

                //Determine base path for the application.  
                var basePath = Directory.GetCurrentDirectory(); //PlatformServices.Default.Application.ApplicationBasePath;
                //Set the comments path for the swagger json and ui.  
                var xmlPath = Path.Combine(basePath, "Host.xml");
                options.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStarted.Register(o =>
            {
                var t = ((Tuple<Startup, IServiceProvider>)o);
                t.Item1.OnApplicationStarted(t.Item2);
            }, Tuple.Create(this, app.ApplicationServices), false);
            applicationLifetime.ApplicationStopping.Register(o =>
            {
                var t = ((Tuple<Startup, IServiceProvider>)o);
                t.Item1.OnApplicationStopping(t.Item2);
            }, Tuple.Create(this, app.ApplicationServices), false);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 &&
                   !System.IO.Path.HasExtension(context.Request.Path.Value) &&
                   !context.Request.Path.Value.StartsWith("/api/"))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MsSystem API V1");
            });

            app.UseRouting();
            // https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.0
            app.UseCors("AllowSameDomain");
            //app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// on app start
        /// </summary>
        private void OnApplicationStarted(IServiceProvider services)
        {
            HttpHelper.HttpClientFactory = services.GetService<IHttpClientFactory>();
            _ = SchedulerCenter.Instance.StartScheduleAsync();
        }

        /// <summary>
        /// on app stop
        /// </summary>
        private void OnApplicationStopping(IServiceProvider services)
        {
            SchedulerCenter.Instance.StopScheduleAsync().Wait();
        }

        /// <summary>
        /// 日志配置
        /// </summary>      
        private void LogConfig()
        {
            //nuget导入
            //Serilog.Extensions.Logging
            //Serilog.Sinks.RollingFile
            //Serilog.Sinks.Async
            var fileSize = 1024 * 1024 * 10;//10M
            var fileCount = 2;
            Log.Logger = new LoggerConfiguration()
                                 .Enrich.FromLogContext()
                                 .MinimumLevel.Debug()
                                 .MinimumLevel.Override("System", LogEventLevel.Information)
                                 .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                                 .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Debug).WriteTo.Async(
                                     a =>
                                     {
                                         a.RollingFile("File/logs/log-{Date}-Debug.txt", fileSizeLimitBytes: fileSize, retainedFileCountLimit: fileCount);
                                     }
                                 ))
                                 .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Information).WriteTo.Async(
                                     a =>
                                     {
                                         a.RollingFile("File/logs/log-{Date}-Information.txt", fileSizeLimitBytes: fileSize, retainedFileCountLimit: fileCount);
                                     }
                                 ))
                                 .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Warning).WriteTo.Async(
                                     a =>
                                     {
                                         a.RollingFile("File/logs/log-{Date}-Warning.txt", fileSizeLimitBytes: fileSize, retainedFileCountLimit: fileCount);
                                     }
                                 ))
                                 .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Error).WriteTo.Async(
                                     a =>
                                     {
                                         a.RollingFile("File/logs/log-{Date}-Error.txt", fileSizeLimitBytes: fileSize, retainedFileCountLimit: fileCount);
                                     }
                                 ))
                                 .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Fatal).WriteTo.Async(
                                     a =>
                                     {
                                         a.RollingFile("File/logs/log-{Date}-Fatal.txt", fileSizeLimitBytes: fileSize, retainedFileCountLimit: fileCount);

                                     }
                                 ))
                                 //所有情况
                                 .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(p => true)).WriteTo.Async(
                                     a =>
                                     {
                                         a.RollingFile("File/logs/log-{Date}-All.txt", fileSizeLimitBytes: fileSize, retainedFileCountLimit: fileCount);
                                     }
                                 )
                                .CreateLogger();
        }
    }
}
