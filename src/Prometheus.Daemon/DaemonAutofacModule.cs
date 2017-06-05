using System.Reflection;
using Autofac;
using MediatR;
using Microsoft.Extensions.Options;
using Prometheus.Core;
using RabbitMQ.Client;

namespace Prometheus.Daemon
{
    public class DaemonAutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<CoreAutofacModule>();

            var assembly = typeof(DaemonAutofacModule).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(assembly)
                .AsSelf()
                .AsImplementedInterfaces();

            base.Load(builder);
        }
    }
}