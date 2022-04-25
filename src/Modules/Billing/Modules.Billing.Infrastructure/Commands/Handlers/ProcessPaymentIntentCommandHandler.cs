using Stripe;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Billing.Infrastructure.DAL.Entities;
using VShop.Modules.Billing.Infrastructure.Configuration;
using VShop.Modules.Billing.Infrastructure.DAL.Repositories.Contracts;

namespace VShop.Modules.Billing.Infrastructure.Commands.Handlers
{
    internal class ProcessPaymentIntentCommandHandler : ICommandHandler<ProcessPaymentIntentCommand>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IIntegrationEventService _integrationEventService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StripeOptions _options;

        public ProcessPaymentIntentCommandHandler
        (
            IPaymentRepository paymentRepository,
            IIntegrationEventService integrationEventService,
            IHttpContextAccessor httpContextAccessor,
            StripeOptions options
        )
        {
            _paymentRepository = paymentRepository;
            _integrationEventService = integrationEventService;
            _httpContextAccessor = httpContextAccessor;
            _options = options;
        }

        public async Task<Result> HandleAsync
        (
            ProcessPaymentIntentCommand command,
            CancellationToken cancellationToken
        )
        {
            HttpRequest httpRequest = _httpContextAccessor.HttpContext?.Request;
            if(httpRequest is null) return Result.InternalServerError("httpRequest cannot be null.");
            
            string json = await new StreamReader(httpRequest.Body).ReadToEndAsync();
            
            StringValues signatureHeader = httpRequest.Headers["Stripe-Signature"];
            Event stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, _options.WebhookSecret);

            if (stripeEvent.Data.Object is not PaymentIntent intent) 
                return Result.ValidationError("Payment intent cannot be null.");

            Guid orderId = Guid.Parse(intent.Metadata["OrderId"]);
            
            Payment payment;
            IIntegrationEvent paymentIntegrationEvent;
            
            switch (stripeEvent.Type)
            {
                case Stripe.Events.PaymentIntentSucceeded:
                    payment = CreatePayment(orderId, PaymentStatus.Success);
                    paymentIntegrationEvent = new PaymentSucceededIntegrationEvent(orderId);
                    break;
                case Stripe.Events.PaymentIntentPaymentFailed:
                    payment = CreatePayment(orderId, PaymentStatus.Failed, intent.LastPaymentError);
                    paymentIntegrationEvent = new PaymentFailedIntegrationEvent(orderId);
                    break;
                default: return Result.ValidationError("Invalid payment event type.");
            }
            
            await _paymentRepository.SaveAsync(payment, cancellationToken);
            await _integrationEventService.SaveEventAsync(paymentIntegrationEvent, cancellationToken);

            return Result.Success;
        }
        
        private static Payment CreatePayment(Guid orderId, PaymentStatus status, StripeError error = null) => new()
        {
            Id = SequentialGuid.Create(),
            OrderId = orderId,
            Type = PaymentType.Transfer,
            Status = status,
            Error = error?.ToJson()
        };
    }
}