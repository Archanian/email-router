using System;
using EmailRouter.Domain.Customers;
using EmailRouter.Domain.Emails;
using EmailRouter.Service.Messages;
using EmailRouter.Service.Validation;
using Xunit;

namespace EmailRouter.Tests.Validation
{
    public class EmailRequestValidatorTests
    {
        private readonly EmailRequestValidator _validator;

        public EmailRequestValidatorTests()
        {
            _validator = new EmailRequestValidator();
        }

        [Fact]
        public void FromAddressMustNotBeEmpty()
        {
            var message = new EmailSendRequest
            {
                Customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    EmailAddress = "customer@test.com"
                },
                SubmittedAt = DateTime.UtcNow,
                EmailPayload = new Email
                {
                    From = string.Empty,
                    To = "recipient@test.com",
                    Subject = "Test email",
                    TextBody = "Test content"
                }
            };

            var result = _validator.Validate(message);

            Assert.NotEmpty(result);
        }

        [Fact]
        public void FromAddressMustMatchCustomersEmail()
        {
            var message = new EmailSendRequest
            {
                Customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    EmailAddress = "customer@test.com"
                },
                SubmittedAt = DateTime.UtcNow,
                EmailPayload = new Email
                {
                    From = "another@test.com",
                    To = "recipient@test.com",
                    Subject = "Test email",
                    TextBody = "Test content"
                }
            };

            var result = _validator.Validate(message);

            Assert.NotEmpty(result);
        }

        [Fact]
        public void ToAddressMustNotBeEmpty()
        {
            var message = new EmailSendRequest
            {
                Customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    EmailAddress = "customer@test.com"
                },
                SubmittedAt = DateTime.UtcNow,
                EmailPayload = new Email
                {
                    From = "customer@test.com",
                    To = string.Empty,
                    Subject = "Test email",
                    TextBody = "Test content"
                }
            };

            var result = _validator.Validate(message);

            Assert.NotEmpty(result);
        }
    }
}