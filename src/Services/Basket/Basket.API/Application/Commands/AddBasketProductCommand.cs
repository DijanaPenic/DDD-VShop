using System;
using MediatR;

using VShop.Services.Basket.API.Application.Commands.Shared;

namespace VShop.Services.Basket.API.Application.Commands
{
    public record AddBasketProductCommand : IRequest<bool>
    {
        public Guid BasketId { get; set; }
        public BasketItemDto BasketItem { get; set; }
    }
}