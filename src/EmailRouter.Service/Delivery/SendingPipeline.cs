using EmailRouter.Domain.Emails;

namespace EmailRouter.Service.Delivery
{
    public class SendingPipeline
    {
        public const string Exchange = "email-delivery";

        public string Name { get; set; }
        public EmailType SendType { get; set; }
        public string QueueName { get; set; }
    }
}