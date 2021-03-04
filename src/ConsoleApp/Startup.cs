using System;
using CasCap.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Debugging;

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
            services.AddControllers();
            services.AddHostedService<WorkerService>();
            var instrumentationKey = _configuration["CasCap:AppInsightsConfig:InstrumentationKey"];
            if (!string.IsNullOrWhiteSpace(instrumentationKey))
            {
                services.AddApplicationInsightsTelemetry(options =>
                {
                    options.EnableDependencyTrackingTelemetryModule = false;
                    options.EnablePerformanceCounterCollectionModule = false;
                    options.DeveloperMode = _env.IsDevelopment();
                    options.InstrumentationKey = _configuration["CasCap:AppInsightsConfig:InstrumentationKey"];
                });
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            SelfLog.Enable(msg => Console.WriteLine(msg));
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}