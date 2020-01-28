using System;
using System.Threading;
using System.Threading.Tasks;
using EmailRouter.Service.Configuration;
using EmailRouter.Service.Handlers;
using EmailRouter.Service.Idempotency;
using EmailRouter.Service.Messages;
using EmailRouter.Service.Parser;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmailRouter.Service
{
    public class Worker : BackgroundService
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly IMessageParser _parser;
        private readonly IMessageDeduplicator _deduplicator;
        private readonly MessageQueueSettings _settings;
        private readonly EmailRequestHandler _emailRequestHandler;
        private readonly ILogger<Worker> _logger;

        public Worker(
            IConnection connection,
            IMessageParser parser,
            IMessageDeduplicator deduplicator,
            MessageQueueSettings settings,
            EmailRequestHandler emailRequestHandler,
            ILogger<Worker> logger)
        {
            _connection = connection;
            _channel = connection.CreateModel();
            _parser = parser;
            _deduplicator = deduplicator;
            _settings = settings;
            _emailRequestHandler = emailRequestHandler;
            _logger = logger;
        }

        public override void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
            
            base.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken cancelToken)
        {
            _logger.LogInformation("Queue worker connected");

            EnsureQueueExists();
            StartConsumingMessages();

            while (!cancelToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancelToken);
            }

            _logger.LogInformation("Queue worker disconnected");
        }

        private void EnsureQueueExists()
        {
            _channel.QueueDeclare(
                queue: _settings.QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        private void StartConsumingMessages()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += HandleMessage;

            _channel.BasicConsume(_settings.QueueName, autoAck: false, consumer);
        }

        private void HandleMessage(object sender, BasicDeliverEventArgs e)
        {
            string messageId = e.BasicProperties.MessageId;
            string consumer = e.ConsumerTag;

            if (_deduplicator.IsDuplicate(messageId, consumer))
            {
                _logger.LogWarning("Duplicate message: {MessageId} (discarding)", messageId);
                return;
            }

            string messageType = e.BasicProperties.Type;
            var message = _parser.Parse(e.Body, messageType);
            if (message == null)
            {
                _logger.LogWarning("Message not parseable: {Type}", messageType);
                return;
            }

            try
            {
                HandleMessage(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Message processing failed: {Error}", ex.Message);
                _channel.BasicNack(e.DeliveryTag, multiple: false, requeue: false);
                return;

                // NOTE: Failure/retry handling is complex and could/should be done much differently in a 
                // real world situation (ie. with retry header, publishing to a retry queue, etc.)
            }

            _channel.BasicAck(e.DeliveryTag, multiple: false);
            _deduplicator.Track(messageId, consumer);
        }

        private void HandleMessage(IMessage message)
        {
            if (message is EmailSendRequest msg)
            {
                _emailRequestHandler.Handle(msg);
            }
            else
            {
                throw new InvalidOperationException($"Message type {message.GetType()} is unsupported");
            }
        }
    }
}
