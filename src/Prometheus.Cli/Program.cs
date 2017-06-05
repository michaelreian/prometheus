using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Core;
using Serilog;

namespace Prometheus.Cli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var settings = new ConfigurationBuilder()
                .AddJsonFile("Settings.json", optional: false, reloadOnChange: true)
                .Build();

            var builder = new ConfigurationBuilder()
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

            containerBuilder.RegisterModule<CoreAutofacModule>();

            containerBuilder.Populate(services);

            var container = containerBuilder.Build();

            var databaseContext = container.Resolve<DatabaseContext>();

            databaseContext.Database.EnsureCreated();
        }
    }
}