using System;

using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public partial class SetDeliveryAddressCommand : MessageContext, ICommand
    {
        public SetDeliveryAddressCommand
        (
            Guid shoppingCartId,
            string city,
            string countryCode,
            string postalCode,
            string stateProvince,
            string streetAddress,
            MessageMetadata metadata
        )
        {
            ShoppingCartId = shoppingCartId;
            City = city;
            CountryCode = countryCode;
            PostalCode = postalCode;
            StateProvince = stateProvince;
            StreetAddress = streetAddress;
            Metadata = metadata;
        }
    }
}