using CasCap.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;
using System.IO;
namespace CasCap
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                //.Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithExceptionDetails()

                //.WriteTo.Console()
                .WriteTo.Console(new ElasticsearchJsonFormatter())
                .WriteTo.Console(new ExceptionAsObjectJsonFormatter())

                .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("appsettings.json", optional: false);
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.AddSerilog();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<WorkerService>();
                });
    }
}