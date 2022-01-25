using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.Application.ValidationAttributes;

namespace VShop.Modules.Sales.API.Models
{
    internal record RemoveShoppingCartProductRequest
    {
        [Required, ProductQuantity]
        public int Quantity { get; init; }
    }
}