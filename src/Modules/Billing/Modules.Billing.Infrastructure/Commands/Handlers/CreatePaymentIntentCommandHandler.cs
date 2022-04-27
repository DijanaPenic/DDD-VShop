using Stripe;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Billing.Infrastructure.Models;
using VShop.Modules.Billing.Infrastructure.DAL.Repositories.Contracts;

using Transfer = VShop.Modules.Billing.Infrastructure.DAL.Entities.Transfer;

namespace VShop.Modules.Billing.Infrastructure.Commands.Handlers
{
    // TODO - missing integration and unit tests
    internal class CreatePaymentIntentCommandHandler : ICommandHandler<CreatePaymentIntentCommand, PaymentIntentInfo>
    {
        private readonly IContext _context;
        private readonly IStripeClient _stripeClient;
        private readonly IPaymentRepository _paymentRepository;

        public CreatePaymentIntentCommandHandler
        (
            IContext context,
            IStripeClient stripeClient,
            IPaymentRepository paymentRepository
        )
        {
            _context = context;
            _stripeClient = stripeClient;
            _paymentRepository = paymentRepository;
        }

        public async Task<Result<PaymentIntentInfo>> HandleAsync
        (
            CreatePaymentIntentCommand command,
            CancellationToken cancellationToken
        )
        {
            Transfer paidPayment = await _paymentRepository.GetPaidPaymentAsync(command.OrderId, cancellationToken);
            if (paidPayment is not null) return Result.ValidationError("The order is already paid.");
            
            PaymentIntentCreateOptions paymentIntentCreateOptions = new()
            {
                Amount = Convert.ToInt32(command.Amount * 100),
                Currency = "USD",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
                ReceiptEmail = command.CustomerEmail,
                Metadata = new Dictionary<string, string>
                {
                    {"OrderId", command.OrderId.ToString()}
                }
            };
            RequestOptions requestOptions = new()
            {
                IdempotencyKey = _context.RequestId.ToString()
            };

            PaymentIntentService paymentIntentService = new(_stripeClient);
            PaymentIntent paymentIntent = await paymentIntentService.CreateAsync
            (
                paymentIntentCreateOptions,
                requestOptions,
                cancellationToken
            );

            if (paymentIntent is null) 
                return Result.ValidationError("Invalid request - payment intent creation failed.");

            return new PaymentIntentInfo(paymentIntent.ClientSecret);
        }
    }
}