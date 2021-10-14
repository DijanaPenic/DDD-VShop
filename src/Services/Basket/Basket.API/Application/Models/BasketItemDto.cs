using System;
using FluentValidation;

namespace VShop.Services.Basket.API.Application.Models
{
    public record BasketItemDto
    {
        public Guid ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
    
    public class BasketItemValidator : AbstractValidator<BasketItemDto> {
        public BasketItemValidator() 
        {
            RuleFor(bi => bi.ProductId).NotEmpty();
            RuleFor(bi => bi.UnitPrice).GreaterThan(0);
            RuleFor(bi => bi.Quantity).GreaterThan(0);
        }
    }
}