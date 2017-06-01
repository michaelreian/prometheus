using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Prometheus.Core;

namespace Prometheus.Core
{
    public static class Extensions
    {
        public static void AddSettings(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddOptions();
            services.Configure<GeneralSettings>(options => { configuration.GetSection("General").Bind(options); });
            services.Configure<RabbitMQConnectionSettings>(options => { configuration.GetSection("RabbitMQConnection").Bind(options); });
        }
    }
}