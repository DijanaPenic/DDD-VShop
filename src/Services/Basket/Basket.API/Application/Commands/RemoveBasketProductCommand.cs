using System;
using MediatR;

namespace VShop.Services.Basket.API.Application.Commands
{
    public record RemoveBasketProductCommand : IRequest<bool>
    {
        public Guid BasketId { get; set; }
        public Guid ProductId { get; set; }
    }
}