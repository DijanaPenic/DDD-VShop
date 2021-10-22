using System;
using FluentValidation;

namespace VShop.Services.ShoppingCarts.API.Models
{
    public record DecreaseShoppingCartProductQuantityRequest
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
    
    public class DecreaseShoppingCartProductQuantityRequestValidator : AbstractValidator<DecreaseShoppingCartProductQuantityRequest> {
        public DecreaseShoppingCartProductQuantityRequestValidator() 
        {
            RuleFor(bi => bi.ProductId).NotEmpty();
            RuleFor(bi => bi.Quantity).GreaterThan(0);
        }
    }
}