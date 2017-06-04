using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Autofac;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace Prometheus.Core
{
    public interface IServiceHost
    {
        int Run();
    }

    public class ServiceHost : IServiceHost
    {
        private readonly IBus bus;
        private readonly ILogger logger;

        public ServiceHost(IBus bus, ILogger logger)
        {
            this.bus = bus;
            this.logger = logger;
        }

        public int Run()
        {
            this.logger.Debug("Hosting starting...");

            var manualResetEvent = new ManualResetEvent(false);

            Console.CancelKeyPress += (sender, eventArgs) => {
                eventArgs.Cancel = true;
                manualResetEvent.Set();
            };

            this.bus.Start();

            this.logger.Debug("Hosting started...");

            Console.WriteLine("Application started. Press Ctrl+C to shut down.");

            manualResetEvent.WaitOne();

            return 0;
        }
    }
}
