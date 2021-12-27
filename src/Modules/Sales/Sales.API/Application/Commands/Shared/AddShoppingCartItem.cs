using System;

namespace VShop.Modules.Sales.API.Application.Commands.Shared
{
    public record AddShoppingCartItem
    {
        public Guid ProductId { get; init; }
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
    }
}