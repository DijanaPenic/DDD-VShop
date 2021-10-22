using System;
using FluentValidation;

namespace VShop.Services.ShoppingCarts.API.Models
{
    public record IncreaseShoppingCartProductQuantityRequest
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
    
    public class IncreaseShoppingCartProductQuantityRequestValidator : AbstractValidator<IncreaseShoppingCartProductQuantityRequest> {
        public IncreaseShoppingCartProductQuantityRequestValidator() 
        {
            RuleFor(bi => bi.ProductId).NotEmpty();
            RuleFor(bi => bi.Quantity).GreaterThan(0);
        }
    }
}