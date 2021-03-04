using CasCap;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.OpenTracing;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;
using System;

//preload basic serilog settings from local json file
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddJsonFile("appsettings.Development.json", optional: true).Build();
var loggerConfiguration = new LoggerConfiguration().ReadFrom.Configuration(config);

//set-up application insights telemetry sink (optional)
var instrumentationKey = config["CasCap:AppInsightsConfig:InstrumentationKey"];
if (!string.IsNullOrWhiteSpace(instrumentationKey))
{
    var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
    telemetryConfiguration.InstrumentationKey = instrumentationKey;
    loggerConfiguration.WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces, restrictedToMinimumLevel: LogEventLevel.Debug);
}

//add all the log enrichers
Log.Logger = loggerConfiguration
    .Enrich.FromLogContext()
    //.Enrich.WithMachineName()
    .Enrich.WithEnvironmentUserName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .Enrich.WithExceptionDetails()
    .Enrich.WithAssemblyName()
    .Enrich.WithOpenTracingContext()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code, applyThemeToRedirectedOutput: true)//local development pretty print console logging 
                                                                                      //.WriteTo.Console(new ElasticsearchJsonFormatter())
                                                                                      //.WriteTo.Console(new ExceptionAsObjectJsonFormatter())//or output as json object for production
                                                                                      //.Filter.ByExcluding($"RequestPath like '/healthz%'")
    .CreateLogger();
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
}
finally
{
    Log.CloseAndFlush();
}