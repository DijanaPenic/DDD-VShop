using System;

using VShop.SharedKernel.Domain;

namespace VShop.Services.Sales.Domain.Events
{
    public record DeliveryAddressSetDomainEvent : IDomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public string City { get; init; }
        public string CountryCode { get; init; }
        public string PostalCode { get; init; }
        public string StateProvince { get; init; }
        public string StreetAddress { get; init; }
    }
}