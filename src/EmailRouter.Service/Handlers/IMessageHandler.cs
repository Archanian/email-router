namespace EmailRouter.Service.Handlers
{
    public interface IMessageHandler<T>
    {
         void Handle(T message);
    }
}