using System.Threading;
using System.Threading.Tasks;
using NodaTime.Serialization.Protobuf;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Billing.Infrastructure.Entities;
using VShop.Modules.Billing.Infrastructure.Services;
using VShop.Modules.Billing.Infrastructure.Repositories;

namespace VShop.Modules.Billing.API.Application.Commands
{
    // TODO - missing integration and unit tests
    public class TransferCommandHandler : ICommandHandler<TransferCommand>
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IIntegrationEventService _billingIntegrationEventService;

        public TransferCommandHandler
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

        public async Task<Result> Handle(IdentifiedCommand<TransferCommand> command, CancellationToken cancellationToken)
        {
            bool isTransferSuccess = await _paymentRepository.IsPaymentSuccessAsync
            (
                command.Data.OrderId,
                PaymentType.Transfer,
                cancellationToken
            );
            if (isTransferSuccess) return Result.Success;


            Result transferResult = await _paymentService.TransferAsync
            (
                command.Data.OrderId,
                command.Data.Amount,
                command.Data.CardTypeId,
                command.Data.CardNumber,
                command.Data.CardSecurityNumber,
                command.Data.CardholderName,
                command.Data.CardExpiration.ToInstant(),
                cancellationToken
            );
            Payment transfer = new()
            {
                Id = SequentialGuid.Create(),
                OrderId = command.Data.OrderId,
                Status = transferResult.IsError ? PaymentStatus.Failed : PaymentStatus.Success,
                Error = transferResult.IsError ? transferResult.Error.ToString() : string.Empty,
                Type = PaymentType.Transfer
            };
            await _paymentRepository.SaveAsync(transfer, cancellationToken);

            IIdentifiedEvent<IIntegrationEvent> paymentIntegrationEvent = new IdentifiedEvent<IIntegrationEvent>
            (
                transferResult.IsError ? new PaymentFailedIntegrationEvent(command.Data.OrderId) : new PaymentSucceededIntegrationEvent(command.Data.OrderId),
                new MessageMetadata(SequentialGuid.Create(), command.Metadata.CorrelationId, command.Metadata.MessageId)
            );

            await _billingIntegrationEventService.SaveEventAsync(paymentIntegrationEvent, cancellationToken);
            
            return transferResult.IsError ? transferResult.Error : Result.Success;
        }
    }
}