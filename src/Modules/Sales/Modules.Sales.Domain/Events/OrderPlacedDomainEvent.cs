using System;

using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class OrderPlacedDomainEvent : IDomainEvent
    {
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
            DeliveryCost = deliveryCost.ToMoney();
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