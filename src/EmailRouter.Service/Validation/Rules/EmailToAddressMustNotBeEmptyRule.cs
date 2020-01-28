using EmailRouter.Service.Messages;

namespace EmailRouter.Service.Validation.Rules
{
    public class EmailToAddressMustNotBeEmptyRule : IValidationRule<EmailSendRequest>
    {
        public ValidationResult Validate(EmailSendRequest message)
        {
            if (string.IsNullOrWhiteSpace(message.EmailPayload.To))
                return ValidationResult.Fail("To address is required");
            
            return ValidationResult.Pass();
        }
    }
}