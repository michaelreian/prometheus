using System;
using System.Threading;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prometheus.Core;
using Serilog;

namespace Prometheus.Daemon
{
    public class Program
    {
        public Program()
        {
        }

        public static int Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (string.IsNullOrEmpty(environment))
            {
                environment = "Development";
            }
            
            var settings = new ConfigurationBuilder()
                .AddJsonFile($"Settings.{environment}.json", optional: true)
                .AddJsonFile("Settings.json", optional: false, reloadOnChange: true)
                .Build();

            var builder = new ConfigurationBuilder()
                .AddJsonFile($"Settings.{environment}.json", optional: true)
                .AddJsonFile("Settings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .AddEntityFrameworkConfig(options =>
                {
                    options.UseNpgsql(settings.GetConnectionString("DefaultConnection"));
                });

            var configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var services = new ServiceCollection();

            services.AddSettings(configuration);

            services.AddMediatR();

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<DaemonAutofacModule>();

            containerBuilder.Populate(services);

            var container = containerBuilder.Build();

            var host = new ServiceHostBuilder()
                .Build(container);

            return host.Run();
        }
    }
}