using EmailRouter.Service.Messages;

namespace EmailRouter.Service.Queue
{
    public interface IQueuePublisher
    {
        void PublishDirect(string exchange, string queue, IMessage message);
    }
}