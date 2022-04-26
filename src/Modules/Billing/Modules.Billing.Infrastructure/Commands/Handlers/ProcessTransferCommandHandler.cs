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

using Transfer = VShop.Modules.Billing.Infrastructure.DAL.Entities.Transfer;

namespace VShop.Modules.Billing.Infrastructure.Commands.Handlers
{
    internal class ProcessTransferCommandHandler : ICommandHandler<ProcessTransferCommand>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IIntegrationEventService _integrationEventService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StripeOptions _options;

        public ProcessTransferCommandHandler
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
            ProcessTransferCommand command,
            CancellationToken cancellationToken
        )
        {
            HttpRequest httpRequest = _httpContextAccessor.HttpContext?.Request;
            if(httpRequest is null) return Result.InternalServerError("httpRequest cannot be null.");
            
            Event stripeEvent;
            try
            {
                string json = await new StreamReader(httpRequest.Body).ReadToEndAsync();

                StringValues signatureHeader = httpRequest.Headers["Stripe-Signature"];
                stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, _options.WebhookSecret);
            }
            catch (StripeException ex)
            {
                return Result.ValidationError($"Stripe event creation failed: {ex.Message}");
            }
            
            return stripeEvent.Data.Object switch
            {
                PaymentIntent paymentIntent => await HandlePaymentAsync(paymentIntent, stripeEvent.Type, cancellationToken),
                Charge charge => await HandleRefundAsync(charge, stripeEvent.Type, cancellationToken),
                _ => Result.Success
            };
        }

        private async Task<Result> HandlePaymentAsync
        (
            PaymentIntent paymentIntent,
            string eventType,
            CancellationToken cancellationToken
        )
        {
            Transfer payment;
            IIntegrationEvent paymentIntegrationEvent;

            switch (eventType)
            {
                case Stripe.Events.PaymentIntentSucceeded:
                    payment = CreatePayment(TransferStatus.Success, paymentIntent);
                    paymentIntegrationEvent = new PaymentSucceededIntegrationEvent(payment.OrderId);
                    break;
                case Stripe.Events.PaymentIntentPaymentFailed:
                    payment = CreatePayment(TransferStatus.Failed, paymentIntent);
                    paymentIntegrationEvent = new PaymentFailedIntegrationEvent(payment.OrderId);
                    break;
                default: return Result.ValidationError("Invalid payment event type.");
            }
            
            await _paymentRepository.SaveAsync(payment, cancellationToken);
            await _integrationEventService.SaveEventAsync(paymentIntegrationEvent, cancellationToken);

            return Result.Success;
        }
        
        private async Task<Result> HandleRefundAsync
        (
            Charge charge,
            string eventType,
            CancellationToken cancellationToken
        )
        {
            Transfer refund;
            
            switch (eventType, charge.Status)
            {
                case (Stripe.Events.ChargeRefunded, "succeeded"):
                    refund = CreateRefund(TransferStatus.Success, charge);
                    break;
                case (Stripe.Events.ChargeRefundUpdated, "failed"):
                    refund = CreateRefund(TransferStatus.Failed, charge);
                    break;
                default: return Result.ValidationError("Invalid refund event type.");
            }
            
            await _paymentRepository.SaveAsync(refund, cancellationToken);

            return Result.Success;
        }

        private static Transfer CreatePayment(TransferStatus status, PaymentIntent paymentIntent) 
            => new()
            {
                Id = SequentialGuid.Create(),
                OrderId = Guid.Parse(paymentIntent.Metadata["OrderId"]),
                Type = TransferType.Payment,
                Status = status,
                Error = paymentIntent.LastPaymentError?.ToJson(),
                IntentId = paymentIntent.Id
            };
        
        private static Transfer CreateRefund(TransferStatus status, Charge charge) 
            => new()
            {
                Id = SequentialGuid.Create(),
                OrderId = Guid.Parse(charge.Metadata["OrderId"]),
                Type = TransferType.Refund,
                Status = status,
                Error = charge.FailureMessage,
                IntentId = charge.PaymentIntentId
            };
    }
}