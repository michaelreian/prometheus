using Autofac;
using Microsoft.Extensions.Configuration;

namespace Prometheus.Core
{
    public class ServiceHostBuilder
    {
        public IServiceHost Build(IContainer container)
        {
            return container.Resolve<IServiceHost>();
        }
    }
}