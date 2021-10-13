using System;
using MediatR;
using FluentValidation;

namespace VShop.Services.Basket.API.Application.Commands
{
    public record CreateBasketCommand : IRequest<bool>
    {
        public Guid CustomerId { get; set; }

        public int CustomerDiscount { get; set; }

        public BasketItem[] BasketItems { get; set; }
    }
    
    public record BasketItem
    {
        public Guid ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateBasketCommandValidator : AbstractValidator<CreateBasketCommand>
    {
        public CreateBasketCommandValidator()
        {
            RuleFor(c => c.CustomerId).NotEmpty();
            RuleFor(c => c.CustomerDiscount).GreaterThanOrEqualTo(0);
            RuleForEach(c => c.BasketItems).SetValidator(new BasketItemValidator());
        }
    }
    
    public class BasketItemValidator : AbstractValidator<BasketItem> {
        public BasketItemValidator() {
            RuleFor(bi => bi.ProductId).NotEmpty();
            RuleFor(bi => bi.UnitPrice).GreaterThan(0);
            RuleFor(bi => bi.Quantity).GreaterThan(0);
        }
    }
}