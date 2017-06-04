using System;
using System.Reflection;
using Autofac;
using MediatR;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Serilog;

namespace Prometheus.Core
{
    public class CoreAutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = typeof(CoreAutofacModule).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(assembly)
                .AsSelf()
                .AsImplementedInterfaces();

            builder.Register(x => Log.Logger).As<ILogger>().SingleInstance();

            builder.RegisterType<Bus>().As<IBus>().SingleInstance();

            builder.Register(x =>
            {
                var settings = x.Resolve<IOptions<RabbitMQConnectionSettings>>();
                return new ConnectionFactory
                {
                    HostName = settings.Value.HostName,
                    Port = settings.Value.Port,
                    UserName = settings.Value.UserName,
                    Password = settings.Value.Password
                };
            }).As<IConnectionFactory>();

            base.Load(builder);
        }
    }
}