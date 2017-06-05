using System;
using System.Threading;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prometheus.Core;
using Serilog;

namespace Prometheus.Daemon
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("Settings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .AddEnvironmentVariables();

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