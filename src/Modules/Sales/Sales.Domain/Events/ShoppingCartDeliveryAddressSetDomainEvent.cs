using System;

using VShop.SharedKernel.Messaging.Events;

namespace VShop.Modules.Sales.Domain.Events
{
    public record ShoppingCartDeliveryAddressSetDomainEvent : DomainEvent
    {
        public Guid ShoppingCartId { get; }
        public string City { get; }
        public string CountryCode { get; }
        public string PostalCode { get; }
        public string StateProvince { get; }
        public string StreetAddress { get; }

        public ShoppingCartDeliveryAddressSetDomainEvent
        (
            Guid shoppingCartId,
            string city,
            string countryCode,
            string postalCode,
            string stateProvince,
            string streetAddress
        )
        {
            ShoppingCartId = shoppingCartId;
            City = city;
            CountryCode = countryCode;
            PostalCode = postalCode;
            StateProvince = stateProvince;
            StreetAddress = streetAddress;
        }
    }
}