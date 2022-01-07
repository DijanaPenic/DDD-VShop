using System.ComponentModel.DataAnnotations;

using VShop.SharedKernel.Application.ValidationAttributes;

namespace VShop.Modules.Sales.API.Models
{
    public record SetShoppingCartProductPriceRequest : BaseRequest
    {
        [Required, Price]
        public decimal UnitPrice { get; init; }
    }
}