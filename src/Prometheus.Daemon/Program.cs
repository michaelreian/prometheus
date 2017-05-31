using System;
using System.Threading;
using Prometheus.Core;

namespace Prometheus.Daemon
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var host = new ServiceHost();

            return host.Run();
        }
    }
}