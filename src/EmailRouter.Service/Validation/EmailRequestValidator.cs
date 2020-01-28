using System.Collections.Generic;
using System.Linq;
using EmailRouter.Service.Messages;
using EmailRouter.Service.Validation.Rules;

namespace EmailRouter.Service.Validation
{
    public class EmailRequestValidator : IValidator<EmailSendRequest>
    {
        public IEnumerable<ValidationResult> Validate(EmailSendRequest message)
        {
            return GetValidationRules()
                .Select(x => x.Validate(message))
                .Where(x => !x.Success);
        }

        private IEnumerable<IValidationRule<EmailSendRequest>> GetValidationRules()
        {
            return new IValidationRule<EmailSendRequest>[]
            {
                new EmailFromAddressMustNotBeEmptyRule(),
                new EmailFromAddressMustMatchCustomerRule(),
                new EmailToAddressMustNotBeEmptyRule()
            };
        }
    }
}