using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.Application.ValidationAttributes;

namespace VShop.Modules.Sales.API.Models
{
    internal record AddShoppingCartProductRequest
    {
        [Required, Price]
        public decimal UnitPrice { get; init; }
        
        [Required, ProductQuantity]
        public int Quantity { get; init; }
    }
}