using Autofac;
using Microsoft.Extensions.Configuration;

namespace Prometheus.Core
{
    public class ServiceHostBuilder
    {
        private readonly IConfigurationRoot configuration;

        public ServiceHostBuilder(IConfigurationRoot configuration)
        {
            this.configuration = configuration;
        }

        public IServiceHost Build(IContainer container)
        {
            return new ServiceHost(container);
        }
    }
}