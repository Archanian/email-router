using EmailRouter.Service.Messages;

namespace EmailRouter.Service.Validation.Rules
{
    public class EmailFromAddressMustMatchCustomerRule : IValidationRule<EmailSendRequest>
    {
        public ValidationResult Validate(EmailSendRequest message)
        {
            if (message.EmailPayload.From != message.Customer.EmailAddress)
                return ValidationResult.Fail("From address must match customer's email address");

            return ValidationResult.Pass();
        }
    }
}