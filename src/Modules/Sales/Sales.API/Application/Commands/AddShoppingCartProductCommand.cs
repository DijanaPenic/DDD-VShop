using System;
using OneOf.Types;

using VShop.SharedKernel.Application.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public record AddShoppingCartProductCommand : ICommand<Success>
    {
        public Guid ShoppingCartId { get; set; }
        public ShoppingCartItemDto ShoppingCartItem { get; set; }
    }
}