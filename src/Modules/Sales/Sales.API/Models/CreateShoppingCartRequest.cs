using System;
using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.Application.ValidationAttributes;

namespace VShop.Modules.Sales.API.Models
{
    internal record CreateShoppingCartRequest
    {
        [Required, EntityId]
        public Guid ShoppingCartId { get; init; }
        
        [Required, EntityId]
        public Guid CustomerId { get; init; }
        
        [Required, Discount]
        public int CustomerDiscount { get; init; }
        
        public ShoppingCartProductRequestDto[] ShoppingCartItems { get; init; }
        
        public record ShoppingCartProductRequestDto
        {
            [Required, EntityId]
            public Guid ProductId { get; init; }
        
            [Required, Price]
            public decimal UnitPrice { get; init; }
        
            [Required, ProductQuantity]
            public int Quantity { get; init; }
        }
    }
}