using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.API.ValidationAttributes;

namespace VShop.Modules.Sales.API.Models
{
    public record AddShoppingCartProductRequest
    {
        [Required, Price]
        public decimal UnitPrice { get; init; }
        
        [Required, ProductQuantity]
        public int Quantity { get; init; }
    }
}