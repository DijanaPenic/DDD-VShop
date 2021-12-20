namespace VShop.Modules.Sales.API.Models
{
    public record AddShoppingCartProductRequest
    {
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
    }
    
    // FluentValidation is not needed: https://enterprisecraftsmanship.com/posts/validate-commands-cqrs/
    // public class AddShoppingCartProductRequestValidator : AbstractValidator<AddShoppingCartProductRequest> {
    //     public AddShoppingCartProductRequestValidator() 
    //     {
    //         RuleFor(sci => sci.UnitPrice).GreaterThan(0);
    //         RuleFor(sci => sci.Quantity).GreaterThan(0);
    //     }
    // }
}