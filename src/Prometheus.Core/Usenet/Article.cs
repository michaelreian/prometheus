using System.Collections.Generic;

namespace Prometheus.Core.Usenet
{
    public class Article
    {
        public Article(IEnumerable<Header> headers, List<string> body)
        {
            Headers = headers;
            Body = body;
        }

        public IEnumerable<Header> Headers { get; private set; }

        public List<string> Body { get; private set; }
    }
}