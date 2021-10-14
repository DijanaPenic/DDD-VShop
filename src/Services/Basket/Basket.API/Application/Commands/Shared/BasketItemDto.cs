using System;

namespace VShop.Services.Basket.API.Application.Commands.Shared
{
    public record BasketItemDto
    {
        public Guid ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}