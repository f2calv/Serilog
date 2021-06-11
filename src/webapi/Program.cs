using CasCap;
using CasCap.Models;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Core;
using Serilog.Enrichers.OpenTracing;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.File;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.Redis.List;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
//load connection strings
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddJsonFile($"appsettings.{environment}.json", optional: true).Build();
//set some serilog settings from local json file (but rely more on configuration-as-code)
var loggerConfiguration = new LoggerConfiguration().ReadFrom.Configuration(configuration);
var appInsightsConfig = configuration.GetSection($"{nameof(CasCap)}:{nameof(AppInsightsConfig)}").Get<AppInsightsConfig>();
if (appInsightsConfig == null) throw new Exception($"Unable to load {nameof(AppInsightsConfig)} configuration, exiting early...");
var connectionStrings = configuration.GetSection(nameof(ConnectionStrings)).Get<ConnectionStrings>();
if (connectionStrings == null) throw new Exception($"Unable to load {nameof(ConnectionStrings)} configuration, exiting early...");

//https://github.com/serilog/serilog-sinks-applicationinsights
if (!string.IsNullOrWhiteSpace(appInsightsConfig.InstrumentationKey))
{
    var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
    telemetryConfiguration.InstrumentationKey = appInsightsConfig.InstrumentationKey;
    loggerConfiguration.WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces, restrictedToMinimumLevel: LogEventLevel.Debug);
}

//https://github.com/serilog/serilog-sinks-mssqlserver
if (!string.IsNullOrWhiteSpace(connectionStrings.mssql))
{
    //if (!IsSqlServerOnline(connectionStrings.mssql_check))
    //    Log.Error("Unable to connect to MSSQL :(");
    //else
    loggerConfiguration.WriteTo.MSSqlServer(
        connectionString: connectionStrings.mssql,
        sinkOptions: new MSSqlServerSinkOptions { TableName = "LogEvents", AutoCreateSqlTable = true });
}
else
    Log.Warning("No MSSQL connection string, skipping...");

//https://github.com/serilog/serilog-sinks-seq
if (!string.IsNullOrWhiteSpace(connectionStrings.seq))
    loggerConfiguration.WriteTo.Seq(connectionStrings.seq);
else
    Log.Warning("No Seq connection string, skipping...");

//https://github.com/serilog/serilog-sinks-elasticsearch
if (!string.IsNullOrWhiteSpace(connectionStrings.elasticsearch))
{
    loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(connectionStrings.elasticsearch))
    {
        //IndexFormat = "workerservice-{0:yyyy.MM.dd}",
        //IndexFormat = AppDomain.CurrentDomain.FriendlyName + "-{0:yyyy.MM}",
        //IndexFormat = "AdminLogs-{0:yyyy.MM.dd}",
        IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
        AutoRegisterTemplate = true,
        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
        //OverwriteTemplate = true,
        //RegisterTemplateFailure = RegisterTemplateRecovery.IndexToDeadletterIndex,
        EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                           EmitEventFailureHandling.RaiseCallback |
                           EmitEventFailureHandling.ThrowException |
                           EmitEventFailureHandling.WriteToFailureSink,
        FailureCallback = e =>
        {
            Log.Error("Unable to submit event {MessageTemplate}", e.MessageTemplate);
            //Console.WriteLine($"Unable to submit event " + e.MessageTemplate);
        },
#pragma warning disable CS0618 // Type or member is obsolete
        FailureSink = new FileSink("failures.log", new JsonFormatter(), null),
#pragma warning restore CS0618 // Type or member is obsolete
        BufferCleanPayload = (failingEvent, statuscode, exception) =>
        {
            dynamic e = JObject.Parse(failingEvent);
            var d = new Dictionary<string, object>
           {
                { "@timestamp", e["@timestamp"]},
                { "level", "Error"},
                { "message", "Error: " + e.message},
                { "messageTemplate", e.messageTemplate},
                { "failingStatusCode", statuscode},
                { "failingException", exception}
           };
            return JsonConvert.SerializeObject(d);
        },
        MinimumLogEventLevel = LogEventLevel.Verbose,
        //CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
        //https://github.com/serilog/serilog-sinks-elasticsearch/issues/184
        CustomFormatter = new EsPropertyTypeNamedFormatter()
    });
}
else
    Log.Warning("No Elasticsearch connection string, skipping...");

//https://github.com/chriswill/serilog-sinks-azureblobstorage
//https://github.com/serilog/serilog-sinks-azuretablestorage
if (!string.IsNullOrWhiteSpace(connectionStrings.azurestorageaccount))
{
    loggerConfiguration.WriteTo.AzureBlobStorage(connectionString: connectionStrings.azurestorageaccount,
        restrictedToMinimumLevel: LogEventLevel.Debug,
        storageContainerName: AppDomain.CurrentDomain.FriendlyName,
        storageFileName: "{yyyy}/{MM}/{dd}/log.txt"
        );

    loggerConfiguration.WriteTo.AzureTableStorage(connectionStrings.azurestorageaccount,
        storageTableName: AppDomain.CurrentDomain.FriendlyName);
}
else
    Log.Warning("No azurestorageaccount connection string, skipping...");

if (false)
{
    //loggerConfiguration.WriteTo.AzureAnalytics(workspaceId: < id removed >,
    //    authenticationId: < id removed >,
    //    logName: "wibble123",
    //    restrictedToMinimumLevel: LogEventLevel.Debug,
    //    //logBufferSize:5,
    //    batchSize: 10
    //    );
}

if (!string.IsNullOrWhiteSpace(connectionStrings.redis))
    loggerConfiguration.WriteTo.RedisList(connectionStrings.redis, AppDomain.CurrentDomain.FriendlyName);

//https://github.com/serilog/serilog-sinks-console
if (true)
{
    loggerConfiguration.WriteTo.Console(theme: AnsiConsoleTheme.Code, applyThemeToRedirectedOutput: true);//local development pretty print console logging
    //loggerConfiguration.WriteTo.Console(outputTemplate: "{Timestamp:HH:mm} [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}");
    //loggerConfiguration.WriteTo.Console(new ElasticsearchJsonFormatter());
    //loggerConfiguration.WriteTo.Console(new ExceptionAsObjectJsonFormatter());//or output as json object for production+filebeat
    //loggerConfiguration.WriteTo.Console(new JsonFormatter());
    //loggerConfiguration.WriteTo.Console(new CompactJsonFormatter());
}

//https://github.com/serilog/serilog-sinks-file
if (true)
{
    loggerConfiguration.WriteTo.File($"{AppDomain.CurrentDomain.FriendlyName}.log", rollingInterval: RollingInterval.Day, buffered: true);
    loggerConfiguration.WriteTo.File(new JsonFormatter(), $"{AppDomain.CurrentDomain.FriendlyName}.json.log", rollingInterval: RollingInterval.Day, buffered: true);
}

var levelSwitch = new LoggingLevelSwitch();//this would have to be a singleton or something and passed in?

//add all the log enrichers
Log.Logger = loggerConfiguration
    .MinimumLevel.ControlledBy(levelSwitch)
    .Enrich.FromLogContext()
    //.Enrich.WithMachineName()
    .Enrich.WithEnvironmentUserName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .Enrich.WithExceptionDetails()
    .Enrich.WithAssemblyName()
    .Enrich.WithOpenTracingContext()
    .Enrich.WithProperty("ApplicationContext", AppDomain.CurrentDomain.FriendlyName)
    .Enrich.WithProperty("Version", typeof(Startup).Assembly.GetName().Version)//const enricher
    .Enrich.With(new ThreadIdEnricher())//dynamic enricher

    .Destructure.ByTransforming<TestObj>(
        r => new { dt = r.utcNow, sid = r.id.ToString(), wibble = "wobble" })

    .Filter.ByExcluding($"RequestPath like '/healthz%'")
    .CreateLogger();
var result = 0;
try
{
    Log.Information("Starting {AppName}", AppDomain.CurrentDomain.FriendlyName);
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostContext, config) => { })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        })
        .UseSerilog()
        .Build().Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "{AppName} terminated unexpectedly", AppDomain.CurrentDomain.FriendlyName);
    result = 1;
}
finally
{
    Log.CloseAndFlush();
}
return result;

bool IsSqlServerOnline(string connectionString, int timeout = 5)
{
    var connectionString2 = $"{connectionString};Connection Timeout={timeout}";
    using (var connection = new SqlConnection(connectionString2))
    {
        try
        {
            connection.Open();
            return true;
        }
        catch (SqlException)
        {
            return false;
        }
    }
}

class ThreadIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
            "ThreadId", Thread.CurrentThread.ManagedThreadId));
    }
}

//https://github.com/serilog/serilog-sinks-elasticsearch/issues/184
internal class EsPropertyTypeNamedFormatter : ElasticsearchJsonFormatter
{
    // Use a property writer that can change the property names and values before writing to ES
    protected override void WritePropertiesValues(IReadOnlyDictionary<string, LogEventPropertyValue> properties, TextWriter output)
    {
        string precedingDelimiter = "";
        foreach (KeyValuePair<string, LogEventPropertyValue> property in properties)
        {
            char type;
            object value;

            // Modify property name
            if (property.Value is ScalarValue asScalar)
            {
                if (asScalar.Value is DateTime || asScalar.Value is DateTimeOffset)
                {
                    type = 'D';
                    value = asScalar.Value;
                }
                else if (asScalar.Value is long || asScalar.Value is int || asScalar.Value is short || asScalar.Value is byte ||
                         asScalar.Value is ulong || asScalar.Value is uint || asScalar.Value is ushort || asScalar.Value is sbyte)
                {
                    type = 'I';
                    value = asScalar.Value;
                }
                else if (asScalar.Value is float || asScalar.Value is double || asScalar.Value is decimal)
                {
                    type = 'F';
                    value = asScalar.Value;
                }
                else
                {
                    type = 'S';
                    value = asScalar.Value?.ToString();
                }
            }
            // TODO: Support sequences, problem: Sequences can contain elements of different types
            // OTOH: all properties in ES are sequences already, so it'd be great to utilize that -- some way of saying "if all elements are datetimes; store as datetimes"
            //else if (property.Value is SequenceValue asSequence)
            //{
            //    // Check types
            //    Type type = null;
            //    foreach (var item in asSequence.Elements)
            //    {

            //    }
            //}
            else
            {
                // Convert to string
                type = 'S';
                value = property.Value?.ToString();
            }

            string key = type + property.Key;
            WriteJsonProperty(key, value, ref precedingDelimiter, output);
        }
    }
}