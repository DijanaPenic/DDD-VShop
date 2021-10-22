using System;
using OneOf.Types;

using VShop.SharedKernel.Infrastructure.Commands;

namespace VShop.Services.Basket.API.Application.Commands
{
    public record RemoveBasketProductCommand : ICommand<Success>
    {
        public Guid BasketId { get; set; }
        public Guid ProductId { get; set; }
    }
}