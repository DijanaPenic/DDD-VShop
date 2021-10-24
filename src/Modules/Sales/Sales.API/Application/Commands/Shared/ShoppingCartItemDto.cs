using System;

namespace VShop.Services.Sales.API.Application.Commands.Shared
{
    public record ShoppingCartItemDto
    {
        public Guid ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}