namespace Prometheus.Core.Usenet
{
    internal static class HelperExtensions
    {

        public static string WithBrackets(this string value)
        {
            return string.Format("<{0}>", value.Trim('<', '>'));
        }
    }
}