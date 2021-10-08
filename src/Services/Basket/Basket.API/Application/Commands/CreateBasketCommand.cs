using System;
using MediatR;
using FluentValidation;

namespace VShop.Services.Basket.API.Application.Commands
{
    public record CreateBasketCommand : IRequest<bool>
    {
        public Guid CustomerId { get; set; }
    }
    
    public class CreateBasketCommandValidator : AbstractValidator<CreateBasketCommand>
    {
        public CreateBasketCommandValidator()
        {
            RuleFor(cb => cb.CustomerId).NotEmpty();
        }
    }
}