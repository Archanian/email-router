using EmailRouter.Domain.Emails;
using EmailRouter.Service.Messages;

namespace EmailRouter.Service.Delivery
{
    public class DeliveryCategorizer : IDeliveryCategorizer
    {
        public SendingPipeline Categorize(EmailSendRequest message)
        {
            // TODO: Implement logic determining seinding pipeline based on message properties

            return new SendingPipeline
            {
                Name = "Transactional",
                SendType = EmailType.Transactional,
                QueueName = "delivery-transactional"
            };
        }
    }
}