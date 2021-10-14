using System;
using MediatR;

using VShop.Services.Basket.API.Application.Commands.Shared;

namespace VShop.Services.Basket.API.Application.Commands
{
    public record AddBasketItemCommand : IRequest<bool>
    {
        public Guid CustomerId { get; set; }
        public BasketItemDto BasketItem { get; set; }
    }
}