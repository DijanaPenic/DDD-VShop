using System;
using FluentValidation;

namespace VShop.Services.ShoppingCarts.API.Models
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
            RuleFor(c => c.CustomerId).NotEmpty();
            RuleFor(c => c.CustomerDiscount).GreaterThanOrEqualTo(0);
            RuleForEach(c => c.ShoppingCartItems).SetValidator(new CreateShoppingCartProductRequestValidator());
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
            RuleFor(bi => bi.ProductId).NotEmpty();
            RuleFor(bi => bi.UnitPrice).GreaterThan(0);
            RuleFor(bi => bi.Quantity).GreaterThan(0);
        }
    }
    
}