using System;

namespace Prometheus.Core.Usenet
{
    public class Overview
    {
        public long ArticleNo { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public DateTime Date { get; set; }
        public string References { get; set; }
        public string MessageID { get; set; }
        public int Bytes { get; set; }
        public int Lines { get; set; }
        public string XRef { get; set; }
    }
}