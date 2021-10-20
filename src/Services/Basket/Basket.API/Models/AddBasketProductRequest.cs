using System;
using FluentValidation;

namespace VShop.Services.Basket.API.Models
{
    public record AddBasketProductRequest
    {
        public Guid ProductId { get; init; }
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
    }
    
    public class AddBasketProductRequestValidator : AbstractValidator<AddBasketProductRequest> {
        public AddBasketProductRequestValidator() 
        {
            RuleFor(bi => bi.ProductId).NotEmpty();
            RuleFor(bi => bi.UnitPrice).GreaterThan(0);
            RuleFor(bi => bi.Quantity).GreaterThan(0);
        }
    }
}