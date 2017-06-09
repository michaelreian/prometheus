namespace Prometheus.Core.Usenet
{
    public class Header
    {
        private Header(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Key, Value);
        }

        public static Header Create(string line)
        {
            var parts = line.Split(':');
            return new Header(parts[0], parts[1].Trim(' '));
        }
    }
}