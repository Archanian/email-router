namespace EmailRouter.Service.Idempotency
{
    public class MessageDeduplicator : IMessageDeduplicator
    {
        public bool IsDuplicate(string messageId, string consumer)
        {
            // We would typically lookup messageId + consumer in storage to determine if 
            // this message has already been processed (Redis is usually a good fit)
            return false;
        }

        public void Track(string messageId, string consumer)
        {
            // Store messageId + consumer
        }
    }
}