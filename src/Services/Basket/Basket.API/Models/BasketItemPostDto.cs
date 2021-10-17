using System;
using FluentValidation;

namespace VShop.Services.Basket.API.Models
{
    public record BasketItemPostDto
    {
        public Guid ProductId { get; init; }
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
    }
    
    public class BasketItemPostDtoValidator : AbstractValidator<BasketItemPostDto> {
        public BasketItemPostDtoValidator() 
        {
            RuleFor(bi => bi.ProductId).NotEmpty();
            RuleFor(bi => bi.UnitPrice).GreaterThan(0);
            RuleFor(bi => bi.Quantity).GreaterThan(0);
        }
    }
}