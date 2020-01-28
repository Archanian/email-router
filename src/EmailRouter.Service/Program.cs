using System;
using System.Threading;
using EmailRouter.Service.Configuration;
using EmailRouter.Service.Delivery;
using EmailRouter.Service.Handlers;
using EmailRouter.Service.Idempotency;
using EmailRouter.Service.Messages;
using EmailRouter.Service.Parser;
using EmailRouter.Service.Queue;
using EmailRouter.Service.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace EmailRouter.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<IConnection>(CreateRabbitMQConnection);
                    services.AddSingleton<MessageQueueSettings>();
                    services.AddTransient<IMessageParser, MessageParser>();
                    services.AddTransient<IMessageDeduplicator, MessageDeduplicator>();
                    services.AddTransient<IDeliveryCategorizer, DeliveryCategorizer>();
                    services.AddTransient<IQueuePublisher, QueuePublisher>();
                    services.AddTransient<IValidator<EmailSendRequest>, EmailRequestValidator>();
                    services.AddTransient<EmailRequestHandler>();
                    services.AddHostedService<Worker>();
                });

        private static IConnection CreateRabbitMQConnection(IServiceProvider provider)
        {
            var settings = provider.GetRequiredService<MessageQueueSettings>();
            
            var factory = new ConnectionFactory
            {
                HostName = settings.Hostname,
                UserName = settings.Username,
                Password = settings.Password
            };

            try
            {
                return factory.CreateConnection();
            }
            catch (BrokerUnreachableException)
            {
                Thread.Sleep(3000); // simple retry mechanism, obviously could be improved with multiple retries, exponential backoff etc
                return factory.CreateConnection();
            }
        }
    }
}
