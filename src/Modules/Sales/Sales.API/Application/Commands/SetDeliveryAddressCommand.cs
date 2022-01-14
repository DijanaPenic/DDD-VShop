using System;

using VShop.SharedKernel.Messaging.Commands;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class SetDeliveryAddressCommand : Command, IBaseCommand
    {
        public SetDeliveryAddressCommand
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