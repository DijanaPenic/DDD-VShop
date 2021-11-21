using System;
using FluentValidation;

namespace VShop.Modules.Billing.API.Models
{
    public record InitiatePaymentRequest
    {
        public Guid OrderId { get; init; }
        public int CardTypeId { get; init; } // TODO - need to check this property
        public string CardNumber { get; init; }
        public string CardSecurityNumber { get; init; }
        public string CardholderName { get; init; }
        public DateTime CardExpiration { get; init; }
    }
    
    public class InitiatePaymentRequestValidator : AbstractValidator<InitiatePaymentRequest>
    {
        public InitiatePaymentRequestValidator()
        {
            RuleFor(pr => pr.OrderId).NotEmpty();
            RuleFor(pr => pr.CardTypeId).NotEmpty(); // TODO 
            RuleFor(pr => pr.CardNumber).NotEmpty();
            RuleFor(pr => pr.CardSecurityNumber).NotEmpty();
            RuleFor(pr => pr.CardholderName).NotEmpty();
            RuleFor(pr => pr.CardExpiration).NotEmpty();
        }
    }
}