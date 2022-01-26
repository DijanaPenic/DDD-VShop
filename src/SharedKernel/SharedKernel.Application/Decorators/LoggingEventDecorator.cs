using MediatR;
using Serilog;

using VShop.SharedKernel.Infrastructure.Events.Contracts;

// TODO - remove MediatR dependencies. 
namespace VShop.SharedKernel.Application.Decorators
{
    public class LoggingEventDecorator<TEvent> : INotificationHandler<TEvent> 
        where TEvent : IBaseEvent
    {
        private readonly ILogger _logger;
        private readonly INotificationHandler<TEvent> _inner;

        public LoggingEventDecorator(ILogger logger, INotificationHandler<TEvent> inner)
        {
            _logger = logger;
            _inner = inner;
        }

        public async Task Handle(TEvent @event, CancellationToken cancellationToken)
        {
            string eventTypeName = @event.GetType().Name;

            _logger.Information
            (
                "Handling event {EventName} ({@Event})",
                eventTypeName, @event
            );

            await _inner.Handle(@event, cancellationToken);
        }
    }
}