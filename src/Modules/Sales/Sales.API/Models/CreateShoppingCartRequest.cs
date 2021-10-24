using System;
using FluentValidation;

namespace VShop.Modules.Sales.API.Models
{
    public record CreateShoppingCartRequest
    {
        public Guid CustomerId { get; init; }
        public int CustomerDiscount { get; init; }
        public CreateShoppingCartProductRequest[] ShoppingCartItems { get; init; }
    }
    
    public class CreateShoppingCartRequestValidator : AbstractValidator<CreateShoppingCartRequest>
    {
        public CreateShoppingCartRequestValidator()
        {
            RuleFor(sc => sc.CustomerId).NotEmpty();
            RuleFor(sc => sc.CustomerDiscount).GreaterThanOrEqualTo(0);
            RuleForEach(sc => sc.ShoppingCartItems).SetValidator(new CreateShoppingCartProductRequestValidator());
        }
    }
    
    public record CreateShoppingCartProductRequest
    {
        public Guid ProductId { get; init; }
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
    }
    
    public class CreateShoppingCartProductRequestValidator : AbstractValidator<CreateShoppingCartProductRequest> {
        public CreateShoppingCartProductRequestValidator() 
        {
            RuleFor(sci => sci.ProductId).NotEmpty();
            RuleFor(sci => sci.UnitPrice).GreaterThan(0);
            RuleFor(sci => sci.Quantity).GreaterThan(0);
        }
    }
    
}