using CasCap.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SerilogTimings;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace CasCap.Services
{
    public class WorkerService : BackgroundService
    {
        readonly ILogger _logger;

        public WorkerService(ILogger<WorkerService> logger) => _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Hello, Serilog!");

                var utcNow = DateTime.UtcNow;

                _logger.LogInformation($"Worker running at: {utcNow} <--- string interpolation means no structured logging!");

                _logger.LogInformation("Worker running at: {utcNow}", utcNow);

                _logger
                    //annoying namespace conflicts...
                    //.ForContext(nameof(Environment.MachineName), Environment.MachineName)//example use of ForContext to include properties and have terser messages
                    .LogInformation("Worker running at: {utcNow}", utcNow);

                var fruit1 = new[] { "Apple", "Pear", "Orange" };
                _logger.LogInformation("In my array bowl I have {@Fruit}", fruit1);

                var fruit2 = new Dictionary<string, int> { { "Apple", 1 }, { "Pear", 5 } };
                _logger.LogInformation("In my dictionary bowl I have {@Fruit}", fruit2);

                var sensorInput = new { Latitude = 25, Longitude = 134 };
                _logger.LogInformation("Processing {@SensorInput}", sensorInput);

                var obj1 = new TestObj();
                _logger.LogDebug("here is my test object {@obj1}", obj1);


                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);

                var position = new { Latitude = 25, Longitude = 134 };
                var elapsedMs = 34;

                _logger.LogInformation("Processed {@Position} in {Elapsed} ms.", position, elapsedMs);

                try
                {
                    string test = null;
                    var indexOf = test.IndexOf(':');
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "indexof failed");
                }


                //play with SerilogTimings...
                var orderId = 123;
                var customerId = 123;

                using (Serilog.Context.LogContext.PushProperty(nameof(customerId), customerId))//lets add additional info to each nested logging request
                {
                    using (Operation.Time("Submitting payment for {orderId}", orderId))
                    {
                        // Timed block of code goes here
                        await Task.Delay(2_000, stoppingToken);
                    }

                    using (var op = Operation.Begin("Retrieving order details for orderId {orderId}", orderId))
                    {
                        // Timed block of code goes here
                        await Task.Delay(2_000, stoppingToken);
                        op.Complete();
                    }

                    using (var op = Operation.Begin("Queuing notifications for orderId {orderId}", orderId))
                    {
                        var notificationCount = 3;
                        // Timed block of code goes here
                        await Task.Delay(2_000, stoppingToken);
                        op.Complete(nameof(notificationCount), notificationCount);
                    }
                }
            }
        }
    }
}