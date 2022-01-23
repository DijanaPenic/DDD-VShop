using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.API.ValidationAttributes;

namespace VShop.Modules.Sales.API.Models
{
    public record RemoveShoppingCartProductRequest
    {
        [Required, ProductQuantity]
        public int Quantity { get; init; }
    }
}