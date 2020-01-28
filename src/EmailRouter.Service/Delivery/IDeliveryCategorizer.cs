using EmailRouter.Service.Messages;

namespace EmailRouter.Service.Delivery
{
    public interface IDeliveryCategorizer
    {
        SendingPipeline Categorize(EmailSendRequest message);
    }
}