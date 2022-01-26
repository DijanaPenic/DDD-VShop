using System.Threading;
using System.Threading.Tasks;
using NodaTime.Serialization.Protobuf;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Billing.Infrastructure.Entities;
using VShop.Modules.Billing.Infrastructure.Services;
using VShop.Modules.Billing.Infrastructure.Repositories;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;

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

        public async Task<Result> Handle(TransferCommand command, CancellationToken cancellationToken)
        {
            bool isTransferSuccess = await _paymentRepository.IsPaymentSuccessAsync
            (
                command.OrderId,
                PaymentType.Transfer,
                cancellationToken
            );
            if (isTransferSuccess) return Result.Success;


            Result transferResult = await _paymentService.TransferAsync
            (
                command.OrderId,
                command.Amount.DecimalValue,
                command.CardTypeId,
                command.CardNumber,
                command.CardSecurityNumber,
                command.CardholderName,
                command.CardExpiration.ToInstant(),
                cancellationToken
            );
            Payment transfer = new()
            {
                Id = SequentialGuid.Create(),
                OrderId = command.OrderId,
                Status = transferResult.IsError ? PaymentStatus.Failed : PaymentStatus.Success,
                Error = transferResult.IsError ? transferResult.Error.ToString() : string.Empty,
                Type = PaymentType.Transfer
            };
            await _paymentRepository.SaveAsync(transfer, cancellationToken);

            MessageMetadata metadata = new
            (
                SequentialGuid.Create(),
                command.Metadata.MessageId,
                command.Metadata.CorrelationId
            );

            IIntegrationEvent paymentIntegrationEvent = transferResult.IsError
                ? new PaymentFailedIntegrationEvent(command.OrderId, metadata)
                : new PaymentSucceededIntegrationEvent(command.OrderId, metadata);
            
            await _billingIntegrationEventService.SaveEventAsync(paymentIntegrationEvent, cancellationToken);
            
            return transferResult.IsError ? transferResult.Error : Result.Success;
        }
    }
}