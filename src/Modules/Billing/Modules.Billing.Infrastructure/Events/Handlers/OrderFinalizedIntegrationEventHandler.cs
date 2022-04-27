using Stripe;

using VShop.Modules.Sales.Integration.Events;
using VShop.Modules.Billing.Infrastructure.DAL.Repositories.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;

using Transfer = VShop.Modules.Billing.Infrastructure.DAL.Entities.Transfer;

namespace VShop.Modules.Billing.Infrastructure.Events.Handlers
{
    internal class OrderFinalizedIntegrationEventHandler : IEventHandler<OrderFinalizedIntegrationEvent>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IStripeClient _stripeClient;
        private readonly IContext _context;

        public OrderFinalizedIntegrationEventHandler
        (
            IPaymentRepository paymentRepository,
            IStripeClient stripeClient,
            IContext context
        )
        {
            _paymentRepository = paymentRepository;
            _stripeClient = stripeClient;
            _context = context;
        }

        public async Task HandleAsync(OrderFinalizedIntegrationEvent @event, CancellationToken cancellationToken)
        {
            decimal refundAmount = @event.RefundAmount.DecimalValue;
            if (refundAmount is 0) return;

            // TODO - need a better error handling.
            Transfer paidPayment = await _paymentRepository.GetPaidPaymentAsync(@event.OrderId, cancellationToken);
            if (paidPayment is null) throw new Exception("Cannot refund as order has not been paid.");

            RefundCreateOptions refundCreateOptions = new()
            {
                PaymentIntent = paidPayment.IntentId,
                Amount = Convert.ToInt32(refundAmount * 100),
                Metadata = new Dictionary<string, string>
                {
                    {"OrderId", @event.OrderId.ToString()}
                }
            };
            RequestOptions requestOptions = new()
            {
                IdempotencyKey = _context.RequestId.ToString()
            };
            
            RefundService service = new(_stripeClient);
            await service.CreateAsync(refundCreateOptions, requestOptions, cancellationToken);
        }
    }
}