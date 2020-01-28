using EmailRouter.Service.Messages;

namespace EmailRouter.Service.Validation.Rules
{
    public class EmailFromAddressMustNotBeEmptyRule : IValidationRule<EmailSendRequest>
    {
        public ValidationResult Validate(EmailSendRequest message)
        {
            if (string.IsNullOrWhiteSpace(message.EmailPayload.From))
                return ValidationResult.Fail("From address is required");

            return ValidationResult.Pass();
        }
    }
}