using System;
using FluentValidation;

namespace VShop.Services.ShoppingCarts.API.Models
{
    public record CreateShoppingCartRequest
    {
        public Guid CustomerId { get; init; }
        public int CustomerDiscount { get; init; }
        public AddShoppingCartProductRequest[] ShoppingCartItems { get; init; }
    }
    
    public class CreateShoppingCartRequestValidator : AbstractValidator<CreateShoppingCartRequest>
    {
        public CreateShoppingCartRequestValidator()
        {
            RuleFor(c => c.CustomerId).NotEmpty();
            RuleFor(c => c.CustomerDiscount).GreaterThanOrEqualTo(0);
            RuleForEach(c => c.ShoppingCartItems).SetValidator(new AddShoppingCartProductRequestValidator());
        }
    }
}