﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace CasCap.Services
{
    public class WorkerService : BackgroundService
    {
        readonly ILogger<WorkerService> _logger;

        public WorkerService(ILogger<WorkerService> logger) => _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var utcNow = DateTime.UtcNow;

                _logger.LogInformation($"Worker running at: {utcNow}");

                _logger.LogInformation("Worker running at: {utcNow}", utcNow);

                var fruit1 = new[] { "Apple", "Pear", "Orange" };
                _logger.LogInformation("In my bowl I have {Fruit}", fruit1);

                var fruit2 = new Dictionary<string, int> { { "Apple", 1 }, { "Pear", 5 } };
                _logger.LogInformation("In my bowl I have {Fruit}", fruit2);

                var sensorInput = new { Latitude = 25, Longitude = 134 };
                _logger.LogInformation("Processing {@SensorInput}", sensorInput);

                var obj1 = new TestObj();
                _logger.LogDebug("here is my test object {@obj1}", obj1);


                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);


                _logger.LogDebug("Hello, Serilog!");

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

                await Task.Delay(5_000, stoppingToken);
            }
        }
    }
}