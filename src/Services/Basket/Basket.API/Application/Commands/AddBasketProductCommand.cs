using System;
using OneOf.Types;

using VShop.SharedKernel.Infrastructure.Commands;
using VShop.Services.Basket.API.Application.Commands.Shared;

namespace VShop.Services.Basket.API.Application.Commands
{
    public record AddBasketProductCommand : ICommand<Success>
    {
        public Guid BasketId { get; set; }
        public BasketItemDto BasketItem { get; set; }
    }
}