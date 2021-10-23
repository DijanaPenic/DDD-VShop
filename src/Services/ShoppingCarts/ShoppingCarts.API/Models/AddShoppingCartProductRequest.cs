using FluentValidation;

namespace VShop.Services.ShoppingCarts.API.Models
{
    public record AddShoppingCartProductRequest
    {
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
    }
    
    public class AddShoppingCartProductRequestValidator : AbstractValidator<AddShoppingCartProductRequest> {
        public AddShoppingCartProductRequestValidator() 
        {
            RuleFor(sci => sci.UnitPrice).GreaterThan(0);
            RuleFor(sci => sci.Quantity).GreaterThan(0);
        }
    }
}