using System;

using VShop.SharedKernel.Infrastructure.Domain;

namespace VShop.Services.ShoppingCarts.Domain.Events
{
    public record DeliveryAddressSetDomainEvent : IDomainEvent
    {
        public Guid BasketId { get; init; }
        public string City { get; init; }
        public string CountryCode { get; init; }
        public string PostalCode { get; init; }
        public string StateProvince { get; init; }
        public string StreetAddress { get; init; }
    }
}