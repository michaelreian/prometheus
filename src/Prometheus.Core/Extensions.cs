using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Prometheus.Core;

namespace Prometheus.Core
{
    public static class Extensions
    {
        public static void AddSettings(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddOptions();
            services.Configure<GeneralSettings>(options => { configuration.GetSection("General").Bind(options); });
            services.Configure<RabbitMQConnectionSettings>(options => { configuration.GetSection("RabbitMQConnection").Bind(options); });
            services.Configure<ApplicationSettings>(options => { configuration.GetSection("Application").Bind(options); });
        }

        public static string ToQueueName(this Type type)
        {
            return $"{type.FullName.ToLower()}.queue";
        }

        public static string ToExchangeName(this Type type)
        {
            return $"{type.FullName.ToLower()}.exchange";
        }

        public static string ToRoutingKey(this Type type)
        {
            return $"{type.FullName.ToLower()}.route";
        }

        public static byte[] ToBytes<T>(this T o)
        {
            var json = JsonConvert.SerializeObject(o);

            return Encoding.UTF8.GetBytes(json);
        }

        public static object ToObject(this byte[] bytes)
        {
            return bytes.ToObject<object>();
        }

        public static T ToObject<T>(this byte[] bytes)
        {
            return (T)bytes.ToObject(typeof(T));
        }

        public static object ToObject(this byte[] bytes, Type type)
        {
            var json = Encoding.UTF8.GetString(bytes);

            return JsonConvert.DeserializeObject(json, type);
        }
    }
}