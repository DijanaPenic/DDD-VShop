using System;
using OneOf.Types;

using VShop.SharedKernel.Infrastructure.Commands;
using VShop.Services.ShoppingCarts.API.Application.Commands.Shared;

namespace VShop.Services.ShoppingCarts.API.Application.Commands
{
    public record CreateShoppingCartCommand : ICommand<Success<Domain.Models.BasketAggregate.Basket>>
    {
        public Guid CustomerId { get; set; }
        public int CustomerDiscount { get; set; }
        public BasketItemDto[] BasketItems { get; set; }
    }
}