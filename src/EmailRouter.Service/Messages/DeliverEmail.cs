using EmailRouter.Domain.Emails;

namespace EmailRouter.Service.Messages
{
    public class DeliverEmail : IMessage
    {
        public Email EmailPayload { get; set; }
    }
}