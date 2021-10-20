using System;
using FluentValidation;

namespace VShop.Services.Basket.API.Models
{
    public record IncreaseBasketProductQuantityRequest
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
    
    public class IncreaseBasketProductQuantityRequestValidator : AbstractValidator<IncreaseBasketProductQuantityRequest> {
        public IncreaseBasketProductQuantityRequestValidator() 
        {
            RuleFor(bi => bi.ProductId).NotEmpty();
            RuleFor(bi => bi.Quantity).GreaterThan(0);
        }
    }
}