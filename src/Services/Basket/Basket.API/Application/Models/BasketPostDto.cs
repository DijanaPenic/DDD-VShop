using System;
using FluentValidation;

namespace VShop.Services.Basket.API.Application.Models
{
    public record BasketPostDto
    {
        public Guid CustomerId { get; init; }
        public int CustomerDiscount { get; init; }
        public BasketItemPostDto[] BasketItems { get; init; }
    }
    
    
    public class BasketPostDtoValidator : AbstractValidator<BasketPostDto>
    {
        public BasketPostDtoValidator()
        {
            RuleFor(c => c.CustomerId).NotEmpty();
            RuleFor(c => c.CustomerDiscount).GreaterThanOrEqualTo(0);
            RuleForEach(c => c.BasketItems).SetValidator(new BasketItemPostDtoValidator());
        }
    }
}