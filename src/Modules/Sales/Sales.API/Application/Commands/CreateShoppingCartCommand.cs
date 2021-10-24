using System;
using OneOf.Types;

using VShop.SharedKernel.Application.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;
using VShop.Modules.Sales.Domain.Models.ShoppingCartAggregate;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public record CreateShoppingCartCommand : ICommand<Success<ShoppingCart>>
    {
        public Guid CustomerId { get; set; }
        public int CustomerDiscount { get; set; }
        public ShoppingCartItemDto[] ShoppingCartItems { get; set; }
    }
}