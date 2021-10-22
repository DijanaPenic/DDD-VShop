using System;
using OneOf.Types;

using VShop.SharedKernel.Infrastructure.Commands;
using VShop.Services.ShoppingCarts.API.Application.Commands.Shared;

namespace VShop.Services.ShoppingCarts.API.Application.Commands
{
    public record AddShoppingCartProductCommand : ICommand<Success>
    {
        public Guid BasketId { get; set; }
        public BasketItemDto BasketItem { get; set; }
    }
}