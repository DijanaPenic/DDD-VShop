using System;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.Modules.Billing.Infrastructure;
using VShop.Modules.Billing.Infrastructure.Entities;
using VShop.Modules.Billing.Infrastructure.Services;
using VShop.Modules.Billing.Integration.Events;

using VShop.SharedKernel.Integration.Services.Contracts;

namespace VShop.Modules.Billing.API.Application.Commands
{
    public class InitiatePaymentCommandHandler : ICommandHandler<InitiatePaymentCommand>
    {
        private readonly IPaymentService _paymentService;
        private readonly BillingContext _billingContext;
        private readonly IIntegrationEventService _billingIntegrationEventService;

        public InitiatePaymentCommandHandler
        (
            IPaymentService paymentService,
            BillingContext billingContext,
            IIntegrationEventService billingIntegrationEventService
        )
        {
            _paymentService = paymentService;
            _billingContext = billingContext;
            _billingIntegrationEventService = billingIntegrationEventService;
        }

        public async Task<Result> Handle(InitiatePaymentCommand command, CancellationToken cancellationToken)
        {
            Result paymentTransferResult = await _paymentService.TransferAsync
            (
                command.OrderId,
                command.CardTypeId,
                command.CardNumber,
                command.CardSecurityNumber,
                command.CardholderName,
                command.CardExpiration,
                cancellationToken
            );

            bool hasPaymentTransferFailed = paymentTransferResult.IsError(out ApplicationError paymentTransferError);

            _billingContext.Payments.Add(new PaymentTransfer
            {
                OrderId = command.OrderId,
                Status = hasPaymentTransferFailed ? PaymentTransferStatus.Failed : PaymentTransferStatus.Success,
                Error = paymentTransferError?.ToString()
            });

            await _billingContext.SaveChangesAsync(cancellationToken);

            IIntegrationEvent integrationEvent = hasPaymentTransferFailed 
                ? new PaymentFailedIntegrationEvent(command.OrderId) : new PaymentSucceededIntegrationEvent(command.OrderId);

            await _billingIntegrationEventService.AddAndSaveEventAsync
            (
                integrationEvent,
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            
            return hasPaymentTransferFailed ? paymentTransferError : Result.Success;
        }
    }
    
    public record InitiatePaymentCommand : Command
    {
        public Guid OrderId { get; set; }
        public int CardTypeId { get; set; }
        public string CardNumber { get; set; }
        public string CardSecurityNumber { get; set; }
        public string CardholderName { get; set; }
        public Instant CardExpiration { get; set; }
    }
}