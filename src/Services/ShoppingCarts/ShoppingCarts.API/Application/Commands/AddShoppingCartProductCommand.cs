using System;
using OneOf.Types;

using VShop.SharedKernel.Application.Commands;
using VShop.Services.ShoppingCarts.API.Application.Commands.Shared;

namespace VShop.Services.ShoppingCarts.API.Application.Commands
{
    public record AddShoppingCartProductCommand : ICommand<Success>
    {
        public Guid ShoppingCartId { get; set; }
        public ShoppingCartItemDto ShoppingCartItem { get; set; }
    }
}