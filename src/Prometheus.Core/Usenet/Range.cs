namespace Prometheus.Core.Usenet
{
    public class Range
    {
        private readonly long fromArticleNo;

        private long? toArticleNo;

        private Range(long fromArticleNo)
        {
            this.fromArticleNo = fromArticleNo;
        }

        public static Range From(long articleNo)
        {
            return new Range(articleNo);
        }

        public Range To(long articleNo)
        {
            toArticleNo = articleNo;
            return this;
        }

        public override string ToString()
        {
            var range = string.Format((string) "{0}-", (object) fromArticleNo);

            if (toArticleNo.HasValue)
            {
                range += toArticleNo.Value;
            }

            return range;
        }
    }
}