using Serilog;
using System.Threading;
using System.Threading.Tasks;

using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Domain.Events.Reminders;
using VShop.Modules.Billing.Integration.Events;
using VShop.Modules.Catalog.Integration.Events;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;
using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;

namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    public class OrderingProcessManagerHandler : ProcessManagerHandler<OrderingProcessManager>,
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
        
        public Task Handle(IdentifiedEvent<ShoppingCartCheckoutRequestedDomainEvent> @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.Data.OrderId, @event, cancellationToken);
        
        public Task Handle(IdentifiedEvent<OrderPlacedDomainEvent> @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.Data.OrderId, @event, cancellationToken);
        
        public Task Handle(IdentifiedEvent<PaymentGracePeriodExpiredDomainEvent> @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.Data.OrderId, @event, cancellationToken);
        
        public Task Handle(IdentifiedEvent<PaymentSucceededIntegrationEvent> @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.Data.OrderId, @event, cancellationToken);
        
        public Task Handle(IdentifiedEvent<PaymentFailedIntegrationEvent> @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.Data.OrderId, @event, cancellationToken);
        
        public Task Handle(IdentifiedEvent<OrderStatusSetToPaidDomainEvent> @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.Data.OrderId, @event, cancellationToken);
        
        public Task Handle(IdentifiedEvent<OrderStockProcessingGracePeriodExpiredDomainEvent> @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.Data.OrderId, @event, cancellationToken);
        
        public Task Handle(IdentifiedEvent<OrderStockProcessedIntegrationEvent> @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.Data.OrderId, @event, cancellationToken);
        
        public Task Handle(IdentifiedEvent<OrderStatusSetToPendingShippingDomainEvent> @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.Data.OrderId, @event, cancellationToken);
        
        public Task Handle(IdentifiedEvent<ShippingGracePeriodExpiredDomainEvent> @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.Data.OrderId, @event, cancellationToken);

        public Task Handle(IdentifiedEvent<OrderStatusSetToCancelledDomainEvent> @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.Data.OrderId, @event, cancellationToken);
    }
}