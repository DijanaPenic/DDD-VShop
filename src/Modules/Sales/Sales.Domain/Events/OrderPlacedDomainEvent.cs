using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record OrderPlacedDomainEvent //: DomainEvent
    {
        public Guid OrderId { get; }
        public decimal DeliveryCost { get; }
        public int CustomerDiscount { get; }
        public Guid CustomerId { get; }
        public string FirstName { get; }
        public string MiddleName { get; }
        public string LastName { get; }
        public string EmailAddress { get; }
        public string PhoneNumber { get; }
        public string City { get; }
        public string CountryCode { get; }
        public string PostalCode { get; }
        public string StateProvince { get; }
        public string StreetAddress { get; }

        public OrderPlacedDomainEvent
        (
            Guid orderId,
            decimal deliveryCost,
            int customerDiscount,
            Guid customerId,
            string firstName,
            string middleName,
            string lastName,
            string emailAddress,
            string phoneNumber,
            string city,
            string countryCode,
            string postalCode,
            string stateProvince,
            string streetAddress
        )
        {
            OrderId = orderId;
            DeliveryCost = deliveryCost;
            CustomerDiscount = customerDiscount;
            CustomerId = customerId;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            EmailAddress = emailAddress;
            PhoneNumber = phoneNumber;
            City = city;
            CountryCode = countryCode;
            PostalCode = postalCode;
            StateProvince = stateProvince;
            StreetAddress = streetAddress;
        }
    }
}