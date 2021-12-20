using System;

namespace VShop.Modules.Sales.API.Models
{
    public record CreateShoppingCartRequest
    {
        public Guid ShoppingCartId { get; init; }
        public Guid CustomerId { get; init; }
        public int CustomerDiscount { get; init; }
        public CreateShoppingCartProductRequest[] ShoppingCartItems { get; init; }
    }

    public record CreateShoppingCartProductRequest
    {
        public Guid ProductId { get; init; }
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
    }
}