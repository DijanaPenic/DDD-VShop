using NodaTime.Serialization.Protobuf;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Billing.Infrastructure.DAL.Entities;
using VShop.Modules.Billing.Infrastructure.DAL.Repositories.Contracts;
using VShop.Modules.Billing.Infrastructure.Services.Contracts;

namespace VShop.Modules.Billing.Infrastructure.Commands.Handlers
{
    // TODO - missing integration and unit tests
    internal class TransferCommandHandler : ICommandHandler<TransferCommand>
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IIntegrationEventService _integrationEventService;

        public TransferCommandHandler
        (
            IPaymentService paymentService,
            IPaymentRepository paymentRepository,
            IIntegrationEventService integrationEventService
        )
        {
            _paymentService = paymentService;
            _paymentRepository = paymentRepository;
            _integrationEventService = integrationEventService;
        }

        public async Task<Result> HandleAsync(TransferCommand command, CancellationToken cancellationToken)
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
            
            IIntegrationEvent paymentIntegrationEvent = transferResult.IsError
                ? new PaymentFailedIntegrationEvent(command.OrderId)
                : new PaymentSucceededIntegrationEvent(command.OrderId);
            
            await _integrationEventService.SaveEventAsync(paymentIntegrationEvent, cancellationToken);
            
            return transferResult.IsError ? transferResult.Error : Result.Success;
        }
    }
}