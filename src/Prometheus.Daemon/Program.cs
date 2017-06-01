using System;
using System.Threading;
using Autofac;
using Microsoft.Extensions.Configuration;
using Prometheus.Core;

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

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<DaemonAutofacModule>();

            var container = containerBuilder.Build();

            var host = new ServiceHostBuilder(configuration)
                .Build(container);

            return host.Run();
        }
    }
}