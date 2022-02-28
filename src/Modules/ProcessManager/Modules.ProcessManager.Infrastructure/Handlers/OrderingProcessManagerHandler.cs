using Serilog;

using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Catalog.Integration.Events;
using VShop.Modules.ProcessManager.Infrastructure.Messages.Events;
using VShop.Modules.ProcessManager.Infrastructure.Messages.Reminders;
using VShop.Modules.ProcessManager.Infrastructure.ProcessManagers.Ordering;
using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.Modules.ProcessManager.Infrastructure.Handlers
{
    internal class OrderingProcessManagerHandler : ProcessManagerHandler<OrderingProcessManager>,
        IEventHandler<ShoppingCartCheckoutRequestedDomainEvent>,
        IEventHandler<OrderPlacedDomainEvent>,
        IEventHandler<PaymentGracePeriodExpiredDomainEvent>,
        IEventHandler<PaymentSucceededIntegrationEvent>,
        IEventHandler<PaymentFailedIntegrationEvent>,
        IEventHandler<OrderStatusSetToPaidDomainEvent>,
        IEventHandler<OrderStockProcessingGracePeriodExpiredDomainEvent>,
        IEventHandler<OrderStockProcessedIntegrationEvent>,
        IEventHandler<OrderStatusSetToPendingShippingDomainEvent>,
        IEventHandler<ShippingGracePeriodExpiredDomainEvent>,
        IEventHandler<OrderStatusSetToCancelledDomainEvent>
    {
        public OrderingProcessManagerHandler
        (
            IClockService clockService,
            ILogger logger,
            IProcessManagerStore<OrderingProcessManager> processManagerStore
        ) : base(clockService, logger, processManagerStore) { }
        
        public Task HandleAsync(ShoppingCartCheckoutRequestedDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
        
        public Task HandleAsync(OrderPlacedDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
        
        public Task HandleAsync(PaymentGracePeriodExpiredDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
        
        public Task HandleAsync(PaymentSucceededIntegrationEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
        
        public Task HandleAsync(PaymentFailedIntegrationEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
        
        public Task HandleAsync(OrderStatusSetToPaidDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
        
        public Task HandleAsync(OrderStockProcessingGracePeriodExpiredDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
        
        public Task HandleAsync(OrderStockProcessedIntegrationEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
        
        public Task HandleAsync(OrderStatusSetToPendingShippingDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
        
        public Task HandleAsync(ShippingGracePeriodExpiredDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);

        public Task HandleAsync(OrderStatusSetToCancelledDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
    }
}