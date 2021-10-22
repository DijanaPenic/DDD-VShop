using System;
using OneOf.Types;

using VShop.SharedKernel.Infrastructure.Commands;
using VShop.Services.Basket.API.Application.Commands.Shared;

namespace VShop.Services.Basket.API.Application.Commands
{
    public record CreateBasketCommand : ICommand<Success<Domain.Models.BasketAggregate.Basket>>
    {
        public Guid CustomerId { get; set; }
        public int CustomerDiscount { get; set; }
        public BasketItemDto[] BasketItems { get; set; }
    }
}