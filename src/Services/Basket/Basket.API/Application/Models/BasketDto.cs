using System;
using FluentValidation;

namespace VShop.Services.Basket.API.Application.Models
{
    public record BasketDto
    {
        public Guid CustomerId { get; set; }
        public int CustomerDiscount { get; set; }
        public BasketItemDto[] BasketItems { get; set; }
    }
    
    
    public class BasketValidator : AbstractValidator<BasketDto>
    {
        public BasketValidator()
        {
            RuleFor(c => c.CustomerId).NotEmpty();
            RuleFor(c => c.CustomerDiscount).GreaterThanOrEqualTo(0);
            RuleForEach(c => c.BasketItems).SetValidator(new BasketItemValidator());
        }
    }
}