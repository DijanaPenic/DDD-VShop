using System;
using OneOf.Types;

using VShop.SharedKernel.Infrastructure.Commands;

namespace VShop.Services.ShoppingCarts.API.Application.Commands
{
    public record RemoveShoppingCartProductCommand : ICommand<Success>
    {
        public Guid BasketId { get; set; }
        public Guid ProductId { get; set; }
    }
}