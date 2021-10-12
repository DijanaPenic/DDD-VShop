using System;

using VShop.SharedKernel.Infrastructure.Domain;
using VShop.SharedKernel.Infrastructure.Domain.Enums;

namespace VShop.Services.Basket.Domain.Events
{
    public record DeliveryAddressSetDomainEvent : IDomainEvent
    {
        public Guid BasketId { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string PostalCode { get; set; }
        public string StateProvince { get; set; }
        public string StreetAddress { get; set; }
    }
}