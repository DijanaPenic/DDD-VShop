using FluentValidation;

namespace VShop.Modules.Sales.API.Models
{
    public record RemoveShoppingCartProductRequest
    {
        public int Quantity { get; init; }
    }
    
    public class RemoveShoppingCartProductRequestValidator : AbstractValidator<RemoveShoppingCartProductRequest> {
        public RemoveShoppingCartProductRequestValidator() 
        {
            RuleFor(sci => sci.Quantity).GreaterThan(0);
        }
    }
}