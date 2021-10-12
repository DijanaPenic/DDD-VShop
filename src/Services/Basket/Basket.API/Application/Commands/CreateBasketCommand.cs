using System;
using MediatR;
using FluentValidation;

namespace VShop.Services.Basket.API.Application.Commands
{
    public record CreateBasketCommand : IRequest<bool>
    {
        public Guid CustomerId { get; set; }

        public int Discount { get; set; }
    }

    public class CreateBasketCommandValidator : AbstractValidator<CreateBasketCommand>
    {
        public CreateBasketCommandValidator()
        {
            RuleFor(c => c.CustomerId).NotEmpty();
            RuleFor(c => c.Discount).GreaterThanOrEqualTo(0);
        }
    }
}