using FluentValidation;

using VShop.Modules.Billing.Infrastructure.Models;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Billing.Infrastructure.Commands
{
    internal class CreatePaymentIntentCommand : ICommand<PaymentIntentInfo>
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string CustomerEmail { get; set; }
        
        public CreatePaymentIntentCommand
        (
            Guid orderId,
            decimal amount,
            string customerEmail
        )
        {
            OrderId = orderId;
            Amount = amount;
            CustomerEmail = customerEmail;
        }
    }
    
    internal class CreatePaymentIntentCommandValidator : AbstractValidator<CreatePaymentIntentCommand>
    {
        public CreatePaymentIntentCommandValidator()
        {
            RuleFor(pr => pr.OrderId).NotEmpty();
            RuleFor(pr => pr.Amount).NotEmpty().GreaterThan(0);
            RuleFor(pr => pr.CustomerEmail).NotEmpty().EmailAddress();
        }
    }
}