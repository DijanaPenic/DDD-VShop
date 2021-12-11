using System;

namespace VShop.Modules.Sales.API.Application.Commands.Shared
{
    public record ShoppingCartItemCommandDto
    {
        public Guid ProductId { get; set; }
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
    }
}