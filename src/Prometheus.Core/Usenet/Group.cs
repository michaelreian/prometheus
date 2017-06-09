namespace Prometheus.Core.Usenet
{
    public class Group
    {
        public long FirstArticleNo { get; set; }
        public long LastArticleNo { get; set; }
        public long ArticleCount { get; set; }
        public string GroupName { get; set; }
    }
}