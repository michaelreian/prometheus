using System;
using System.Reflection;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Serilog.Context;

namespace Prometheus.Core
{
    public class NotifyConsumersCommand : IRequest<bool>
    {
        public Type MessageType { get; set; }
        public IModel Channel { get; set; }
        public object Consumer { get; set; }
        public BasicDeliverEventArgs Event { get; set; }
    }

    public class NotifyConsumersCommandHandler : IRequestHandler<NotifyConsumersCommand, bool>
    {
        private readonly ILogger logger;

        public NotifyConsumersCommandHandler(ILogger logger)
        {
            this.logger = logger;
        }

        public bool Handle(NotifyConsumersCommand message)
        {
            using (LogContext.PushProperty("ConsumerTag", message.Event.ConsumerTag))
            {
                this.logger.Debug("Received {MessageType} from Exchange: {Exchange} RoutingKey: {RoutingKey}", message.MessageType, message.Event.Exchange, message.Event.RoutingKey);

                var consumerType = message.Consumer.GetType();

                var handleMethod = consumerType.GetMethod("Handle", BindingFlags.Public | BindingFlags.Instance);

                var genericConsumerContextType = typeof(ConsumerContext<>);
                Type[] typeArguments = { message.MessageType };
                var consumerContextType = genericConsumerContextType.MakeGenericType(typeArguments);
                var consumerContext = Activator.CreateInstance(consumerContextType, new object[] { message.Event });

                try
                {
                    handleMethod.Invoke(message.Consumer, new object[] { consumerContext });
                    
                    message.Channel.BasicAck(message.Event.DeliveryTag, false);
                }
                catch (Exception exception)
                {
                    this.logger.Error(exception, "Unable to handle message.");
                }

                return true;
            }
        }
    }
}