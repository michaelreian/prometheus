using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Prometheus.Core
{
    public class QueueSettingsAttribute : Attribute
    {
        public string Name { get; set; } = "";
        public bool Exclusive { get; set; } = false;
        public bool Durable { get; set; } = true;
        public bool AutoDelete { get; set; } = true;
        public Dictionary<string, object> Arguments { get; set; } = null;
    }

    public interface IMessage
    {
        
    }

    public class ConsumerContext<TMessage>
    {
        public TMessage Message { get; set; }

        public ConsumerContext(TMessage message)
        {
            Message = message;
        }
    }

    public interface IConsumer<TMessage> where TMessage : IMessage
    {
        void Handle(ConsumerContext<TMessage> context);
    }

    [QueueSettings]
    public class HelloWorldEvent : IMessage
    {
        public string Name { get; set; }
    }

    public abstract class Consumer<TMessage> : IConsumer<TMessage> where TMessage : IMessage
    {

        protected void Invoke(TMessage message)
        {
            var consumerContext = new ConsumerContext<TMessage>(message);
            this.Handle(consumerContext);
        }

        public abstract void Handle(ConsumerContext<TMessage> context);
    }

    public class HelloWorldEventConsumer : Consumer<HelloWorldEvent>
    {
        public override void Handle(ConsumerContext<HelloWorldEvent> context)
        {
            Console.WriteLine($"Hello, {context.Message.Name}!");
        }
    }

    public interface IBus
    {
        void Initialize();
        void Publish<TMessage>(TMessage message) where TMessage : IMessage;
    }

    public class Bus : IBus
    {
        private readonly IDurableConnection connection;
        private readonly IComponentContext container;

        private IModel channel = null;

        public Bus(IDurableConnection connection, IComponentContext container)
        {
            this.connection = connection;
            this.container = container;
        }

        private void EnsureConnected()
        {
            if (this.channel == null)
            {
                this.channel = this.connection.CreateModel();
            }
        }



        public void Initialize()
        {
            this.EnsureConnected();

            var messages = this.container.Resolve<IMessage[]>();

            Console.WriteLine("Messages: {0}", messages.Length);

            foreach (var message in messages)
            {
                var messageType = message.GetType();

                Console.WriteLine("messageType: {0}", messageType.FullName);

                var queueSettings = messageType.GetTypeInfo().GetCustomAttribute<QueueSettingsAttribute>();

                if (queueSettings == null)
                {
                    queueSettings = new QueueSettingsAttribute();
                }

                var name = queueSettings.Name;

                if (string.IsNullOrEmpty(name))
                {
                    name = messageType.FullName.ToLower();
                }

                this.channel.QueueDeclare(name, queueSettings.Durable, queueSettings.Exclusive, queueSettings.AutoDelete, queueSettings.Arguments);


                var handlers = ((IEnumerable)this.container.Resolve(typeof(IEnumerable<>).MakeGenericType(typeof(IConsumer<>).MakeGenericType(messageType))));

                foreach (var handler in handlers)
                {
                    var handlerType = handler.GetType();

                    var methods = handlerType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

                    Console.WriteLine("handlerType: {0}", handlerType.FullName);

                    var consumer = new EventingBasicConsumer(this.channel);

                    consumer.Received += (sender, model) =>
                    {
                        var body = Encoding.UTF8.GetString(model.Body);

                        var m = JsonConvert.DeserializeObject(body, messageType);

                        var method = handlerType.GetMethod("Invoke", BindingFlags.NonPublic | BindingFlags.Instance);

                        method.Invoke(handler, new [] { m });

                        this.channel.BasicAck(model.DeliveryTag, false);
                    };

                    this.channel.BasicConsume(name, false, consumer);
                }
            }
        }

        public void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            this.EnsureConnected();

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "",
                routingKey: typeof(TMessage).FullName.ToLower(),
                basicProperties: properties,
                body: body);
        }
    }

    public interface IDurableConnection
    {
        IModel CreateModel();
    }

    public class DurableConnection : IDurableConnection
    {
        private readonly IConnectionFactory connectionFactory;

        private IConnection connection = null;

        public DurableConnection(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public IModel CreateModel()
        {
            this.EnsureConnected();
            return this.connection.CreateModel();
        }

        private void EnsureConnected()
        {
            if (connection == null)
            {
                this.connection = this.connectionFactory.CreateConnection();
            }           
        }
    }
}