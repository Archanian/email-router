using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using EmailRouter.Service.Handlers;
using EmailRouter.Service.Queue;
using EmailRouter.Service.Validation;
using EmailRouter.Service.Messages;
using EmailRouter.Service.Delivery;
using Moq;
using Xunit;
using EmailRouter.Domain.Emails;

namespace EmailRouter.Tests
{
    public class EmailRequestHandlerTests
    {
        private readonly Mock<IQueuePublisher> _publisher;
        private readonly Mock<IDeliveryCategorizer> _categorizer;
        private readonly Mock<IValidator<EmailSendRequest>> _validator;
        private readonly EmailRequestHandler _handler;

        public EmailRequestHandlerTests()
        {
            _publisher = new Mock<IQueuePublisher>();
            _categorizer = new Mock<IDeliveryCategorizer>();
            _validator = new Mock<IValidator<EmailSendRequest>>();

            _handler = new EmailRequestHandler(
                _publisher.Object,
                _categorizer.Object,
                _validator.Object,
                NullLogger<EmailRequestHandler>.Instance);
        }

        [Fact]
        public void OnValidationFailure_DeliveryMessageIsNotPublished()
        {
            _validator.Setup(x => x.Validate(It.IsAny<EmailSendRequest>()))
                .Returns(new[] { ValidationResult.Fail("Test failure") });
            
            _handler.Handle(new EmailSendRequest());

            _publisher.Verify(
                x => x.PublishDirect(SendingPipeline.Exchange, It.IsAny<string>(), It.IsAny<IMessage>()),
                Times.Never());
        }

        [Fact]
        public void OnValidationSuccess_DeliveryMessageIsPublished()
        {
            string queue = "delivery-test";

            _validator.Setup(x => x.Validate(It.IsAny<EmailSendRequest>()))
                .Returns(Enumerable.Empty<ValidationResult>());
            
            _categorizer.Setup(x => x.Categorize(It.IsAny<EmailSendRequest>()))
                .Returns(new SendingPipeline
                {
                    Name = "Test Pipeline",
                    SendType = EmailType.Transactional,
                    QueueName = queue
                });
            
            _handler.Handle(new EmailSendRequest());

            _publisher.Verify(
                x => x.PublishDirect(SendingPipeline.Exchange, queue, It.IsAny<IMessage>()),
                Times.Once());
        }
    }
}
