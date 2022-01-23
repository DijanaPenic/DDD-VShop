﻿using System;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Sales.Domain.Events
{
    public partial class ShoppingCartDeliveryAddressSetDomainEvent : MessageContext, IDomainEvent
    {
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