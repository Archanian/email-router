using Microsoft.Extensions.Configuration;

namespace EmailRouter.Service.Configuration
{
    public class MessageQueueSettings
    {
        public string Hostname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }

        public MessageQueueSettings(IConfiguration config)
        {
            var section = config.GetSection("MessageQueue");

            Hostname = section["Hostname"];
            Username = section["Username"];
            Password = section["Password"];
            QueueName = section["QueueName"];
        }
    }
}