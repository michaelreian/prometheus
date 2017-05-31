using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace Prometheus.Core
{
    public class Publisher
    {
        public void Publish(string message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "rabbitmq", Password = "rabbitmq" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                    routingKey: "task_queue",
                    basicProperties: properties,
                    body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }
        }
    }
}
