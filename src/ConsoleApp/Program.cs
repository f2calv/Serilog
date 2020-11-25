﻿using Serilog;
using System;
namespace CasCap
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var log = new LoggerConfiguration()
                .WriteTo.Console()
                //.WriteTo.File("log.txt")
                .WriteTo.File("log.txt",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true)
                .CreateLogger();

            log.Information("Hello, Serilog!");

            var position = new { Latitude = 25, Longitude = 134 };
            var elapsedMs = 34;

            log.Information("Processed {@Position} in {Elapsed} ms.", position, elapsedMs);
            Console.ReadLine();
        }
    }
}