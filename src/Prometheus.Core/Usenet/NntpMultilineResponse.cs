using System.Collections.Generic;

namespace Prometheus.Core.Usenet
{
    public class NntpMultilineResponse : NntpResponse
    {
        internal NntpMultilineResponse(int code, string message, IEnumerable<string> lines) : base(code, message)
        {
            Lines = lines;
        }

        public IEnumerable<string> Lines { get; private set; }
    }
}