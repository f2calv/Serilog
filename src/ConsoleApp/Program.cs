using CasCap.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;
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
                //.WriteTo.Console(new ElasticsearchJsonFormatter())
                .WriteTo.Console(new ExceptionAsObjectJsonFormatter())

                .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) => { })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();//remove existing providers
                    logging.AddSerilog();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<WorkerService>();
                });
    }
}