using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NSwag.AspNetCore;
using Prometheus.Core;

namespace Prometheus.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("Settings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"Settings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddOptions();
            services.Configure<GeneralSettings>(options => { Configuration.GetSection("General").Bind(options); });
            services.Configure<RabbitMQConnectionSettings>(options => { Configuration.GetSection("RabbitMQConnection").Bind(options); });

            var builder = new ContainerBuilder();

            builder.Populate(services);

            builder.RegisterModule<ApiAutofacModule>();

            var container = builder.Build();

            return new AutofacServiceProvider(container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<GeneralSettings> generalSettings)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly, new SwaggerUiOwinSettings
            {
                DefaultPropertyNameHandling = PropertyNameHandling.CamelCase,
                DefaultEnumHandling = EnumHandling.String,
                Version = generalSettings.Value.ApplicationVersion,
                Title = generalSettings.Value.ApplicationName,
                SwaggerRoute = generalSettings.Value.SwaggerRoute,
                SwaggerUiRoute = generalSettings.Value.SwaggerUiRoute,
                IsAspNetCore = true,
                ValidateSpecification = true,
            });

            app.UseMvc();
        }
    }
}
