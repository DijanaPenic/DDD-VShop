using System;
using OneOf.Types;

using VShop.SharedKernel.Application.Commands;

namespace VShop.Services.Sales.API.Application.Commands
{
    public record SetDeliveryAddressCommand : ICommand<Success>
    {
        public Guid ShoppingCartId { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string PostalCode { get; set; }
        public string StateProvince { get; set; }
        public string StreetAddress { get; set; }
    }
}