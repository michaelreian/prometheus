namespace Prometheus.Core
{
    public class GeneralSettings
    {
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
        public string SwaggerRoute { get; set; }
        public string SwaggerUiRoute { get; set; }
        public string Role { get; set; }
    }

    public class ApplicationSettings
    {
        public string Role { get; set; }
    }

    public class RabbitMQConnectionSettings
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}