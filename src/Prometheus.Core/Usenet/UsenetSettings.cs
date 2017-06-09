namespace Prometheus.Core.Usenet
{
    public class UsenetSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Connections { get; set; }
    }
}