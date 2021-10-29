using System;
using OneOf.Types;

using VShop.SharedKernel.Application.Commands;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.API.Application.Commands.Shared;

namespace VShop.Modules.Sales.API.Application.Commands
{
    // TODO - move command classes to handlers
    public record CreateShoppingCartCommand : ICommand<Success<ShoppingCart>>
    {
        public Guid CustomerId { get; set; }
        public int CustomerDiscount { get; set; }
        public ShoppingCartItemDto[] ShoppingCartItems { get; set; }
    }
}