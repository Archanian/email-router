namespace EmailRouter.Service.Idempotency
{
    public interface IMessageDeduplicator
    {
         bool IsDuplicate(string messageId, string consumer);
         void Track(string messageId, string consumer);
    }
}