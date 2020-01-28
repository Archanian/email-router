using EmailRouter.Service.Messages;

namespace EmailRouter.Service.Validation
{
    public interface IValidationRule<T> where T : IMessage
    {
        ValidationResult Validate(T message);
    }
}