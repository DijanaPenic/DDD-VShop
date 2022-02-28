using Serilog;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.PostgresDb.Contracts;
using VShop.SharedKernel.Integration.Services.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.Application.Decorators;

public sealed class TransactionalEventHandlerDecorator<TEvent> : IEventHandler<TEvent>, IDecorator
    where TEvent : class, IBaseEvent
{
    private readonly ILogger _logger;
    private readonly IEventHandler<TEvent> _handler;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIntegrationEventService _integrationEventService;

    public TransactionalEventHandlerDecorator
    (
        ILogger logger,
        IEventHandler<TEvent> handler,
        IUnitOfWork unitOfWork,
        IIntegrationEventService integrationEventService
    )
    {
        _logger = logger;
        _handler = handler;
        _unitOfWork = unitOfWork;
        _integrationEventService = integrationEventService;
    }
    
    public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
    {
        string eventTypeName = @event.GetType().Name;
            
        Guid transactionId = await _unitOfWork.ExecuteAsync(()
            => _handler.HandleAsync(@event, cancellationToken), cancellationToken);
            
        await _integrationEventService.PublishEventsAsync(transactionId, cancellationToken);
            
        _logger.Information
        (
            "Commit transaction {TransactionId} for {EventName}",
            transactionId, eventTypeName
        );
    }
}
