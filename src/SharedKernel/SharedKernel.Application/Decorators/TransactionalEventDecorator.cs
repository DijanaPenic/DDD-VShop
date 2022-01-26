using MediatR;
using Serilog;

using VShop.SharedKernel.PostgresDb.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.Application.Decorators
{
    public class TransactionalEventDecorator<TEvent> : INotificationHandler<TEvent> 
        where TEvent : IBaseEvent
    {
        private readonly ILogger _logger;
        private readonly INotificationHandler<TEvent> _inner;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventService _integrationEventService;

        public TransactionalEventDecorator
        (
            ILogger logger,
            INotificationHandler<TEvent> inner,
            IUnitOfWork unitOfWork,
            IIntegrationEventService integrationEventService
        )
        {
            _logger = logger;
            _inner = inner;
            _unitOfWork = unitOfWork;
            _integrationEventService = integrationEventService;
        }

        public async Task Handle(TEvent @event, CancellationToken cancellationToken)
        {
            string eventTypeName = @event.GetType().Name;

            try
            {
                Guid transactionId = await _unitOfWork.ExecuteAsync(() => 
                    _inner.Handle(@event, cancellationToken), cancellationToken);
                
                await _integrationEventService.PublishEventsAsync(transactionId, cancellationToken);
                
                _logger.Information
                (
                    "Commit transaction {TransactionId} for {EventName}",
                    transactionId, eventTypeName
                );
            }
            catch (Exception ex)
            {
                _logger.Error
                (
                    ex, "Error Handling transaction for {EventName} ({@Event})",
                    eventTypeName, @event
                );

                throw;
            }
        }
    }
}