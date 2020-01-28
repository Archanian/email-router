using System;
using System.Text;
using System.Text.Json;
using EmailRouter.Service.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace EmailRouter.Service.Queue
{
    public class QueuePublisher : IQueuePublisher
    {
        // NOTE: This is a super basic queue publishing implementation just to get things
        // started ... in reality there would be a lot more to establishing reliable message 
        // publishing and more realistic/detailed routing.

        private readonly IConnection _connection;
        private readonly IModel _channel;

        public QueuePublisher(IConnection connection)
        {
            _connection = connection;
            _channel = connection.CreateModel();
        }

        public void PublishDirect(string exchange, string queue, IMessage message)
        {
            _channel.ExchangeDeclare(exchange, type: ExchangeType.Direct);
            _channel.QueueDeclare(queue);

            var messageBody = JsonSerializer.Serialize(message);
            var properties = new BasicProperties()
            {
                MessageId = Guid.NewGuid().ToString(),
                Type = message.GetType().FullName
            };

            _channel.BasicPublish(
                exchange: exchange,
                routingKey: queue,
                basicProperties: properties,
                body: Encoding.UTF8.GetBytes(messageBody));
        }
    }
}