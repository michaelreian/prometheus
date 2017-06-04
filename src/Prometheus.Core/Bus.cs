using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Serilog;

namespace Prometheus.Core
{
    public enum ExchangeType
    {
        Direct,
        Topic,
        Headers,
        Fanout
    }

    public class RouteSettingsAttribute : Attribute
    {
        public string Exchange { get; set; }
        public string Key { get; set; }
        public Dictionary<string, object> Arguments { get; set; } = null;
    }

    public class MessageIDAttribute : Attribute
    {

    }

    public class CorrelationIDAttribute : Attribute
    {

    }

    public class QueueSettingsAttribute : Attribute
    {
        public string Name { get; set; }
        public bool Exclusive { get; set; } = false;
        public bool Durable { get; set; } = true;
        public bool AutoDelete { get; set; } = true;
        public Dictionary<string, object> Arguments { get; set; } = null;
    }

    public class ExchangeSettingsAttribute : Attribute
    {
        public string Name { get; set; }
        public ExchangeType Type { get; set; } = ExchangeType.Direct;
        public bool Durable { get; set; } = false;
        public bool AutoDelete { get; set; } = false;
        public Dictionary<string, object> Arguments { get; set; } = null;
    }

    public interface IMessage
    {
        
    }

    public interface ICommand : IMessage
    {

    }

    public interface IEvent : IMessage
    {

    }

    public class ConsumerContext<TMessage>
    {
        public TMessage Message { get; }
        public BasicDeliverEventArgs Event { get; }

        public ConsumerContext(BasicDeliverEventArgs e)
        {
            this.Message = e.Body.ToObject<TMessage>();
            this.Event = e;
        }
    }

    public interface IConsumer<TMessage> where TMessage : IMessage
    {
        void Handle(ConsumerContext<TMessage> context);
    }


    public class DoSomethingCommand : ICommand
    {
        public string Something { get; set; }
    }

    public class DoSomethingCommandConsumer : Consumer<DoSomethingCommand>
    {
        public override void Handle(ConsumerContext<DoSomethingCommand> context)
        {
            Console.WriteLine($"Doing {context.Message.Something}...");
        }
    }

    [ExchangeSettings(Type = ExchangeType.Fanout)]
    [RouteSettings(Key = "")]
    public class HelloWorldEvent : IEvent
    {
        public string Name { get; set; }
    }

    public class HelloWorldEventConsumer : Consumer<HelloWorldEvent>
    {
        public override void Handle(ConsumerContext<HelloWorldEvent> context)
        {
            Console.WriteLine($"Received {context.Message.Name}. AppID: {context.Event.BasicProperties.AppId}");
        }
    }

    public class PingEvent : IEvent
    {
        public DateTime Timestamp { get; set; }
    }

    public class PingEventConsumer : Consumer<PingEvent>
    {
        public override void Handle(ConsumerContext<PingEvent> context)
        {
            Console.WriteLine($"Pinged in {DateTime.UtcNow.Subtract(context.Message.Timestamp)}");
        }
    }

    public abstract class Consumer<TMessage> : IConsumer<TMessage> where TMessage : IMessage
    {

        protected void Invoke(BasicDeliverEventArgs e)
        {
            var consumerContext = new ConsumerContext<TMessage>(e);
            this.Handle(consumerContext);
        }

        public abstract void Handle(ConsumerContext<TMessage> context);
    }



    public interface IBus
    {
        void Start();
        void Send<TMessage>(TMessage message) where TMessage : IMessage;
    }

    public class Bus : IBus
    {
        private readonly IConnectionFactory connectionFactory;
        private readonly IOptions<ApplicationSettings> applicationSettings;
        private readonly IComponentContext container;
        private readonly ILogger logger;

        private IConnection connection = null;
        private IModel channel = null;

        public Bus(IConnectionFactory connectionFactory, IOptions<ApplicationSettings> applicationSettings,
            IComponentContext container, ILogger logger)
        {
            this.connectionFactory = connectionFactory;
            this.applicationSettings = applicationSettings;
            this.container = container;
            this.logger = logger;
        }

        private void EnsureConnected()
        {
            if (this.connection == null)
            {
                this.logger.Debug("Creating connection...");

                Policy
                    .Handle<BrokerUnreachableException>()
                    .WaitAndRetry(3,
                        attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                        (exception, duration, attempt, context) =>
                        {
                            this.logger.Warning(exception,
                                "Unable to create connection. Attempt #{Attempt} failed. Sleeping for {Duration}ms",
                                attempt, duration.TotalMilliseconds);

                        })
                    .Execute(() =>
                    {
                        this.connection = this.connectionFactory.CreateConnection();
                    });
            }

            if (this.channel == null)
            {
                this.logger.Debug("Creating channel...");
                this.channel = connection.CreateModel();
            }
        }

        private IEnumerable<Type> GetMessageTypes()
        {
            var messages = this.container.Resolve<IMessage[]>();

            return messages.Select(x => x.GetType());
        }

        private IEnumerable GetConsumers(Type message)
        {
            return (IEnumerable) this.container.Resolve(
                typeof(IEnumerable<>).MakeGenericType(typeof(IConsumer<>).MakeGenericType(message)));
        }


        public void Start()
        {
            this.logger.Debug("Bus starting...");

            this.EnsureConnected();

            this.logger.Debug("Registering messages...");

            var messageTypes = this.GetMessageTypes();

            foreach (var messageType in messageTypes)
            {
                this.logger.Debug("Registering {MessageType}", messageType.FullName);

                var queueSettings = messageType.GetTypeInfo().GetCustomAttribute<QueueSettingsAttribute>();

                if (queueSettings == null)
                {
                    queueSettings = new QueueSettingsAttribute();
                }

                var queueName = queueSettings.Name;

                if (string.IsNullOrEmpty(queueName))
                {
                    queueName = messageType.ToQueueName();
                }

                this.channel.QueueDeclare(queueName, queueSettings.Durable, queueSettings.Exclusive, queueSettings.AutoDelete, queueSettings.Arguments);



                var exchangeSettings = messageType.GetTypeInfo().GetCustomAttribute<ExchangeSettingsAttribute>();

                if (exchangeSettings == null)
                {
                    exchangeSettings = new ExchangeSettingsAttribute();
                }

                var exchangeName = exchangeSettings.Name;

                if (string.IsNullOrEmpty(exchangeName))
                {
                    exchangeName = messageType.ToExchangeName();
                }

                var exchangeType = exchangeSettings.Type.ToString().ToLower();

                this.channel.ExchangeDeclare(exchangeName, exchangeType, exchangeSettings.Durable, exchangeSettings.AutoDelete, exchangeSettings.Arguments);



                var route = messageType.GetTypeInfo().GetCustomAttribute<RouteSettingsAttribute>();

                if (route != null)
                {
                    var queue = queueName;
                    var exchange = route.Exchange ?? exchangeName;
                    var routingKey = route.Key ?? messageType.ToRoutingKey();

                    this.logger.Debug("Binding Queue: {Queue} Exchange: {Exchange} RoutingKey: {RoutingKey}", queueName, exchange, routingKey);

                    this.channel.QueueBind(queue, exchange, routingKey, route.Arguments);
                }

                this.logger.Debug("Registering consumers for {MessageType}", messageType.FullName);

                var consumers = this.GetConsumers(messageType);

                foreach (var consumer in consumers)
                {
                    var consumerType = consumer.GetType();

                    this.logger.Debug("Attaching {MessageType} to {ConsumerType}...", messageType.FullName, consumerType.FullName);

                    var basicConsumer = new EventingBasicConsumer(this.channel);

                    var method = consumerType.GetMethod("Invoke", BindingFlags.NonPublic | BindingFlags.Instance);

                    basicConsumer.Received += (sender, e) =>
                    {
                        this.logger.Debug("Received {MessageType} from Exchange: {Exchange} RoutingKey: {RoutingKey}", messageType, e.Exchange, e.RoutingKey);

                        method.Invoke(consumer, new[] {e});

                        this.channel.BasicAck(e.DeliveryTag, false);
                    };


                    this.channel.BasicConsume(queueName, false, basicConsumer);
                }
            }

            this.logger.Debug("Bus started...");
        }

        public void Send<TMessage>(TMessage message) where TMessage : IMessage
        {
            this.EnsureConnected();

            var messageType = typeof(TMessage);

            var route = typeof(TMessage).GetTypeInfo().GetCustomAttribute<RouteSettingsAttribute>();

            var body = message.ToBytes();

            var properties = channel.CreateBasicProperties();
            
            properties.AppId = this.applicationSettings.Value.Role;
            properties.Persistent = true;
            properties.CorrelationId = Guid.NewGuid().ToString();
            properties.MessageId = Guid.NewGuid().ToString();

            var exchange = route?.Exchange ?? messageType.ToExchangeName();
            var routingKey = route?.Key ?? messageType.ToQueueName();

            this.logger.Debug("Sending {Message} to Exchange: {Exchange} RoutingKey: {RoutingKey}", message, exchange, routingKey);

            channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: body
            );
        }
    }
}