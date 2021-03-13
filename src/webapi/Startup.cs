using CasCap.Models;
using CasCap.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using System;
namespace CasCap
{
    public class Startup
    {
        readonly IConfiguration _configuration;
        readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Log.Logger);//Note: this is only needed if we inject Serilog.ILogger instead of the Microsoft abstraction... see TestController
            services.AddControllers();
            services.AddHostedService<WorkerService>();

            var appInsightsConfig = _configuration.GetSection($"{nameof(CasCap)}:{nameof(AppInsightsConfig)}").Get<AppInsightsConfig>();
            if (!string.IsNullOrWhiteSpace(appInsightsConfig.InstrumentationKey))
            {
                services.AddApplicationInsightsTelemetry(options =>
                {
                    options.EnableDependencyTrackingTelemetryModule = false;
                    options.EnablePerformanceCounterCollectionModule = false;
                    options.DeveloperMode = _env.IsDevelopment();
                    options.InstrumentationKey = appInsightsConfig.InstrumentationKey;
                });
            }
        }

        public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            logger.LogDebug("Configure is starting...");
            if (_env.IsDevelopment())
            {
                SelfLog.Enable(msg => Console.WriteLine(msg));
                app.UseDeveloperExceptionPage();
            }
            //app.UseSerilogRequestLogging(options => { options.GetLevel = LogHelper.CustomGetLevel; });
            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            logger.LogDebug("Configure has finished!");
        }
    }

    public static class LogHelper
    {
        public static LogEventLevel CustomGetLevel(HttpContext ctx, double _, Exception ex)
        {
            //return ex != null
            //    ? LogEventLevel.Error
            //    : ctx.Response.StatusCode > 499
            //    ? LogEventLevel.Error
            //    : LogEventLevel.Debug; //Debug instead of Information
            return LogEventLevel.Debug;
        }
    }
}