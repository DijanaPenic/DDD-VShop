using Serilog;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.Application.Decorators;

public sealed class LoggingEventHandlerDecorator<TEvent> : IEventHandler<TEvent>, IDecorator
    where TEvent : class, IBaseEvent
{
    private readonly ILogger _logger;
    private readonly IEventHandler<TEvent> _handler;

    public LoggingEventHandlerDecorator(ILogger logger, IEventHandler<TEvent> handler)
    {
        _logger = logger;
        _handler = handler;
    }
    
    public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
    {
        string eventTypeName = @event.GetType().Name;

        _logger.Information
        (
            "Handling event {EventName} ({@Event})",
            eventTypeName, @event
        );

        await _handler.HandleAsync(@event, cancellationToken);
    }
}