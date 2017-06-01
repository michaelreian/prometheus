﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Autofac;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Prometheus.Core
{
    public interface IServiceHost
    {
        int Run();
    }

    public class ServiceHost : IServiceHost
    {
        private readonly IComponentContext context;

        public ServiceHost(IComponentContext context)
        {
            this.context = context;
        }

        public int Run()
        {
            var manualResetEvent = new ManualResetEvent(false);

            Console.CancelKeyPress += (sender, eventArgs) => {
                eventArgs.Cancel = true;
                manualResetEvent.Set();
            };

            //var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "rabbitmq", Password = "rabbitmq" };
            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    channel.QueueDeclare(queue: "task_queue",
            //        durable: true,
            //        exclusive: false,
            //        autoDelete: false,
            //        arguments: null);

            //    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            //    Console.WriteLine(" [*] Waiting for messages.");

            //    var consumer = new EventingBasicConsumer(channel);
            //    consumer.Received += (model, ea) =>
            //    {
            //        var body = ea.Body;
            //        var message = Encoding.UTF8.GetString(body);
            //        Console.WriteLine(" [x] Received {0}", message);

            //        int dots = message.Split('.').Length - 1;
            //        Thread.Sleep(dots * 1000);

            //        Console.WriteLine(" [x] Done");

            //        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            //    };
            //    channel.BasicConsume(queue: "task_queue",
            //        noAck: false,
            //        consumer: consumer);

            //    Console.WriteLine("Application started. Press Ctrl+C to shut down.");

            //    manualResetEvent.WaitOne();
            //}

            Console.WriteLine("Application started. Press Ctrl+C to shut down.");

            manualResetEvent.WaitOne();

            return 0;
        }
    }
}
