using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using EmailRouter.Service.Delivery;
using EmailRouter.Service.Messages;
using EmailRouter.Service.Queue;
using EmailRouter.Service.Validation;
using Microsoft.Extensions.Logging;

namespace EmailRouter.Service.Handlers
{
    public class EmailRequestHandler : IMessageHandler<EmailSendRequest>
    {
        private readonly IQueuePublisher _publisher;
        private readonly IDeliveryCategorizer _categorizer;
        private readonly IValidator<EmailSendRequest> _validator;
        private readonly ILogger<EmailRequestHandler> _logger;

        public EmailRequestHandler(
            IQueuePublisher publisher,
            IDeliveryCategorizer categorizer,
            IValidator<EmailSendRequest> validator,
            ILogger<EmailRequestHandler> logger)
        {
            _publisher = publisher;
            _categorizer = categorizer;
            _validator = validator;
            _logger = logger;
        }

        public void Handle(EmailSendRequest message)
        {
            _logger.LogDebug("EmailRequestHandler received message {Message}", JsonSerializer.Serialize(message));

            // Validation rules are a hard failure, these will result in the message not being sent
            // The custom should be notified and the failures should also be tracked by the system
            var validationFailures = _validator.Validate(message);
            if (validationFailures.Any())
            {
                _logger.LogDebug(
                    "Message validation failed {Failures}",
                    validationFailures.Select(x => x.Error));

                HandleInvalidRequest(message, validationFailures);
                return;
            }

            // Categorization rules determine which delivery pipeline we should route the email to
            // At this stage we assume the message is valid and we are deciding where to send it
            var deliveryCategory = _categorizer.Categorize(message);

            _logger.LogDebug(
                "Message categorized: {SendType} will publish to {Queue}",
                deliveryCategory.SendType, deliveryCategory.QueueName);

            _publisher.PublishDirect(
                SendingPipeline.Exchange,
                deliveryCategory.QueueName,
                new DeliverEmail
                {
                    EmailPayload = message.EmailPayload
                });
        }

        private void HandleInvalidRequest(EmailSendRequest message, IEnumerable<ValidationResult> validationFailures)
        {
            // We could perform synchronous failure handling here, although that might not be ideal?
            // We could instead publish a message to a failure queue? Picked up by a consumer that can:
            //  - Store the failures to be viewed in Postmark management UI
            //  - Send an alert to the customer (if enabled)
        }
    }
}