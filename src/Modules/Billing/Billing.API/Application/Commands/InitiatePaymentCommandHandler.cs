using System;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Billing.Infrastructure.Entities;
using VShop.Modules.Billing.Infrastructure.Services;
using VShop.Modules.Billing.Infrastructure.Repositories;

namespace VShop.Modules.Billing.API.Application.Commands
{
    public class InitiatePaymentCommandHandler : ICommandHandler<InitiatePaymentCommand>
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IIntegrationEventService _billingIntegrationEventService;

        public InitiatePaymentCommandHandler
        (
            IPaymentService paymentService,
            IPaymentRepository paymentRepository,
            IIntegrationEventService billingIntegrationEventService
        )
        {
            _paymentService = paymentService;
            _paymentRepository = paymentRepository;
            _billingIntegrationEventService = billingIntegrationEventService;
        }

        public async Task<Result> Handle(InitiatePaymentCommand command, CancellationToken cancellationToken)
        {
            bool isPaid = await _paymentRepository.IsOrderPaidAsync(command.OrderId, cancellationToken);
            if (isPaid) return Result.Success;
            
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

            PaymentTransfer payment = new()
            {
                Id = SequentialGuid.Create(),
                OrderId = command.OrderId,
                Status = paymentTransferResult.IsError ? PaymentTransferStatus.Failed : PaymentTransferStatus.Success,
                Error = paymentTransferResult.IsError ? paymentTransferResult.Error.ToString() : string.Empty
            };
            await _paymentRepository.SaveAsync(payment, cancellationToken);

            IIntegrationEvent integrationEvent = paymentTransferResult.IsError 
                ? new PaymentFailedIntegrationEvent(command.OrderId) : new PaymentSucceededIntegrationEvent(command.OrderId);

            integrationEvent.CausationId = command.CausationId;
            integrationEvent.CorrelationId = command.CorrelationId;

            await _billingIntegrationEventService.AddAndSaveEventAsync(integrationEvent, cancellationToken);
            
            return paymentTransferResult.IsError ? paymentTransferResult.Error : Result.Success;
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