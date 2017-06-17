using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Core;
using Serilog;

namespace Prometheus.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseUrls("http://*:8088")
                .Build();

            var logger = host.Services.GetService<ILogger>();

            var bus = host.Services.GetService<IBus>();

            try
            {
                bus.Start();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Unabled to connect to bus. {0}", exception.Message);
            }

            host.Run();
        }
    }
}
