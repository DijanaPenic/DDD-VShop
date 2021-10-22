using System;
using FluentValidation;

namespace VShop.Services.ShoppingCarts.API.Models
{
    public record DecreaseBasketProductQuantityRequest
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
    
    public class DecreaseBasketProductQuantityRequestValidator : AbstractValidator<DecreaseBasketProductQuantityRequest> {
        public DecreaseBasketProductQuantityRequestValidator() 
        {
            RuleFor(bi => bi.ProductId).NotEmpty();
            RuleFor(bi => bi.Quantity).GreaterThan(0);
        }
    }
}