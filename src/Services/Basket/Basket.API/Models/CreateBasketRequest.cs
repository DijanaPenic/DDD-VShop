using System;
using FluentValidation;

namespace VShop.Services.Basket.API.Models
{
    public record CreateBasketRequest
    {
        public Guid CustomerId { get; init; }
        public int CustomerDiscount { get; init; }
        public AddBasketProductRequest[] BasketItems { get; init; }
    }
    
    public class CreateBasketRequestValidator : AbstractValidator<CreateBasketRequest>
    {
        public CreateBasketRequestValidator()
        {
            RuleFor(c => c.CustomerId).NotEmpty();
            RuleFor(c => c.CustomerDiscount).GreaterThanOrEqualTo(0);
            RuleForEach(c => c.BasketItems).SetValidator(new AddBasketProductRequestValidator());
        }
    }
}