using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NSwag.AspNetCore;
using Prometheus.Core;
using Serilog;

namespace Prometheus.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var settings = new ConfigurationBuilder()
                .AddJsonFile("Settings.json", optional: false, reloadOnChange: true)
                .Build();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("Settings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"Settings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .AddEntityFrameworkConfig(options =>
                {
                    options.UseNpgsql(settings.GetConnectionString("DefaultConnection"));
                });

            Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();
        }

        public IConfigurationRoot Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddMvc();

            services.AddMemoryCache();

            services.AddSettings(Configuration);

            services.AddMediatR();

            var builder = new ContainerBuilder();

            builder.Populate(services);
            
            builder.RegisterModule<ApiAutofacModule>();

            var container = builder.Build();

            return new AutofacServiceProvider(container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<GeneralSettings> generalSettings)
        {
            app.UseMiddleware<AddCorrelationIdToLogContextMiddleware>();
            app.UseMiddleware<AddCorrelationIdToResponseMiddleware>();

            app.UseCors(builder =>
                builder.WithOrigins(
                    "http://localhost:8080", 
                    "http://prometheus.mikesoft.com.au", 
                    "https://prometheus.mikesoft.com.au"
            ));

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

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseMvc();
        }
    }
}
