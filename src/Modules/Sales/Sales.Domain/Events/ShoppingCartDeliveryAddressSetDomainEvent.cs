using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartDeliveryAddressSetDomainEvent : DomainEvent
    {
        public Guid ShoppingCartId { get; init; }
        public string City { get; init; }
        public string CountryCode { get; init; }
        public string PostalCode { get; init; }
        public string StateProvince { get; init; }
        public string StreetAddress { get; init; }
    }
}