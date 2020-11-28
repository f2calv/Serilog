using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
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