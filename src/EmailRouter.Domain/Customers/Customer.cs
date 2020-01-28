using System;

namespace EmailRouter.Domain.Customers
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string EmailAddress { get; set; }
    }
}