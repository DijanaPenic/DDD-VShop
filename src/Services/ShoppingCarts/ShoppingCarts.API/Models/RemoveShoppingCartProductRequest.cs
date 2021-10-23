using FluentValidation;

namespace VShop.Services.ShoppingCarts.API.Models
{
    public record RemoveShoppingCartProductRequest
    {
        public int Quantity { get; init; }
    }
    
    public class RemoveShoppingCartProductRequestValidator : AbstractValidator<RemoveShoppingCartProductRequest> {
        public RemoveShoppingCartProductRequestValidator() 
        {
            RuleFor(bi => bi.Quantity).GreaterThan(0);
        }
    }
}