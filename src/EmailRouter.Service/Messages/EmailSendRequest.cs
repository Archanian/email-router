using System;
using EmailRouter.Domain.Customers;
using EmailRouter.Domain.Emails;

namespace EmailRouter.Service.Messages
{
    public class EmailSendRequest : IMessage
    {
        public Customer Customer { get; set; }
        public DateTime SubmittedAt { get; set; }
        public Email EmailPayload { get; set; }
    }
}