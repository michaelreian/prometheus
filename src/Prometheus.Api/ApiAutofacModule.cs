using System.Reflection;
using Autofac;
using MediatR;
using Microsoft.Extensions.Options;
using Prometheus.Core;
using RabbitMQ.Client;

namespace Prometheus.Api
{
    public class ApiAutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = typeof(ApiAutofacModule).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(assembly)
                .AsSelf()
                .AsImplementedInterfaces();

            builder.AddMediatR(assembly);

            base.Load(builder);
        }
    }
}