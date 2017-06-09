using System;

namespace Prometheus.Core.Usenet
{
    public class NntpException : Exception
    {
        public NntpException(string message) : base(message) { }
    }
}