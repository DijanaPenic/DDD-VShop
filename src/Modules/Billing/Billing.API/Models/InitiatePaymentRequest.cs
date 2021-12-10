using System;
using NodaTime;
using FluentValidation;

namespace VShop.Modules.Billing.API.Models
{
    public record InitiatePaymentRequest
    {
        public Guid OrderId { get; init; }
        public int CardTypeId { get; init; }
        public string CardNumber { get; init; }
        public string CardSecurityNumber { get; init; }
        public string CardholderName { get; init; }
        public Instant CardExpiration { get; init; }
    }
    
    public class InitiatePaymentRequestValidator : AbstractValidator<InitiatePaymentRequest>
    {
        public InitiatePaymentRequestValidator()
        {
            RuleFor(pr => pr.OrderId).NotEmpty();
            RuleFor(pr => pr.CardTypeId).NotEmpty();
            RuleFor(pr => pr.CardNumber).NotEmpty();
            RuleFor(pr => pr.CardSecurityNumber).NotEmpty();
            RuleFor(pr => pr.CardholderName).NotEmpty();
            RuleFor(pr => pr.CardExpiration).NotEmpty();
        }
    }
}