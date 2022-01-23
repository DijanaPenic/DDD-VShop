using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.API.ValidationAttributes;

namespace VShop.Modules.Sales.API.Models
{
    public record SetShoppingCartProductPriceRequest
    {
        [Required, Price]
        public decimal UnitPrice { get; init; }
    }
}