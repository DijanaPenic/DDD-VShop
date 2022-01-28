using System;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Commands
{
    public partial class SetDeliveryAddressCommand : ICommand
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