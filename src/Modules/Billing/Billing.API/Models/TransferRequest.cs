using System;
using System.Runtime.Intrinsics.Arm;
using NodaTime;
using FluentValidation;

namespace VShop.Modules.Billing.API.Models
{
    public record TransferRequest : BaseRequest
    {
        public Guid OrderId { get; init; }
        public decimal Amount { get; init; }
        public int CardTypeId { get; init; }
        public string CardNumber { get; init; }
        public string CardSecurityNumber { get; init; }
        public string CardholderName { get; init; }
        public Instant CardExpiration { get; init; }
    }
    
    public class TransferRequestValidator : AbstractValidator<TransferRequest>
    {
        public TransferRequestValidator()
        {
            RuleFor(pr => pr.MessageId).NotEmpty();
            RuleFor(pr => pr.CorrelationId).NotEmpty();
            RuleFor(pr => pr.OrderId).NotEmpty();
            RuleFor(pr => pr.Amount).NotEmpty().GreaterThan(0);
            RuleFor(pr => pr.CardTypeId).NotEmpty();
            RuleFor(pr => pr.CardNumber).NotEmpty();
            RuleFor(pr => pr.CardSecurityNumber).NotEmpty();
            RuleFor(pr => pr.CardholderName).NotEmpty();
            RuleFor(pr => pr.CardExpiration).NotEmpty();
        }
    }
}