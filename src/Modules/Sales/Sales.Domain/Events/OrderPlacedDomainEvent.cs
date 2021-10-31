﻿using System;

using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.Domain.Events
{
    public record OrderPlacedDomainEvent : IDomainEvent
    {
        public Guid OrderId { get; init; }
        public decimal DeliveryCost { get; init; }
        public decimal TotalDiscount { get; init; }
        public Guid CustomerId { get; init; }
        public string FirstName { get; init; }
        public string MiddleName { get; init; }
        public string LastName { get; init; }
        public string EmailAddress { get; init; }
        public string PhoneNumber { get; init; }
        public string City { get; init; }
        public string CountryCode { get; init; }
        public string PostalCode { get; init; }
        public string StateProvince { get; init; }
        public string StreetAddress { get; init; }
    }
}