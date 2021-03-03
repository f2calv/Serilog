using CasCap;
using CasCap.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using System;
using System.Threading;

//var configuration = new ConfigurationBuilder()
//    .AddJsonFile("appsettings.json")
//    .Build();

var levelSwitch = new LoggingLevelSwitch();//this would have to be a singleton or something and passed in?

//levelSwitch.MinimumLevel = LogEventLevel.Verbose;
//log.Verbose("This will now be logged");
//var environment = "Development";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(levelSwitch)
    .Enrich.WithProperty("Version", "1.0.0")//const enricher
    .Enrich.With(new ThreadIdEnricher())//dynamic enricher
    //.Enrich.WithMachineName()
    .Enrich.WithExceptionDetails()

    .Destructure.ByTransforming<TestObj>(
        r => new { dt = r.utcNow, sid = r.id.ToString(), wibble = "wobble" })

    //.WriteTo.Console()
    .WriteTo.Console(
        outputTemplate: "{Timestamp:HH:mm} [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}")
    .WriteTo.File("log.txt",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true)
    //.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    //{
    //    //IndexFormat = "workerservice-{0:yyyy.MM.dd}",
    //    //IndexFormat = AppDomain.CurrentDomain.FriendlyName + "-{0:yyyy.MM}",
    //    //IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
    //    AutoRegisterTemplate = true,
    //    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
    //    //IndexFormat = "AdminLogs-{0:yyyy.MM.dd}",
    //    //OverwriteTemplate = true,
    //    //RegisterTemplateFailure = RegisterTemplateRecovery.IndexToDeadletterIndex,
    //    EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
    //                       EmitEventFailureHandling.RaiseCallback |
    //                       EmitEventFailureHandling.ThrowException |
    //                       EmitEventFailureHandling.WriteToSelfLog,
    //    FailureCallback = e =>
    //    {
    //        Console.WriteLine("Unable to submit event " + e.MessageTemplate);
    //    }
    //})
    //.WriteTo.AzureAnalytics(workspaceId: < id removed >,
    //    authenticationId: < id removed >,
    //    logName: "wibble123",
    //    restrictedToMinimumLevel: LogEventLevel.Debug,
    //    //logBufferSize:5,
    //    batchSize: 10
    //    )
    //.WriteTo.AzureBlobStorage(connectionString: < connection str removed >,
    //    restrictedToMinimumLevel: LogEventLevel.Debug,
    //    storageContainerName: "test",//AppDomain.CurrentDomain.FriendlyName,
    //    storageFileName: "{yyyy}/{MM}/{dd}/log.txt"
    //    )
    //.WriteTo.AzureTableStorage(connectionString: <connection str removed>,
    //    storageTableName: AppDomain.CurrentDomain.FriendlyName)
    //.ReadFrom.Configuration(configuration)
    .CreateLogger();
try
{
    Log.Information("Starting {AppName}", AppDomain.CurrentDomain.FriendlyName);
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<WorkerService>();
        })
        .UseSerilog()
        .Build().Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "{AppName} terminated unexpectedly", AppDomain.CurrentDomain.FriendlyName);
}
finally
{
    Log.CloseAndFlush();
}

class ThreadIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
            "ThreadId", Thread.CurrentThread.ManagedThreadId));
    }
}