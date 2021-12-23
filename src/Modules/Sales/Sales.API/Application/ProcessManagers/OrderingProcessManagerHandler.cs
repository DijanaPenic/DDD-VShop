using Serilog;
using System.Threading;
using System.Threading.Tasks;

using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Billing.Integration.Events;
using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;

namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    public class OrderingProcessManagerHandler : ProcessManagerHandler<OrderingProcessManager>,
        IDomainEventHandler<ShoppingCartCheckoutRequestedDomainEvent>,
        IDomainEventHandler<OrderPlacedDomainEvent>,
        IIntegrationEventHandler<PaymentSucceededIntegrationEvent>,
        IIntegrationEventHandler<PaymentFailedIntegrationEvent>,
        IDomainEventHandler<ShippingGracePeriodExpiredDomainEvent>,
        IDomainEventHandler<PaymentGracePeriodExpiredDomainEvent>,
        IDomainEventHandler<OrderStatusSetToCancelledDomainEvent>
    {
        public OrderingProcessManagerHandler
        (
            IClockService clockService,
            ILogger logger,
            IProcessManagerStore<OrderingProcessManager> processManagerStore
        ) : base(clockService, logger, processManagerStore) { }
        
        public Task Handle(ShoppingCartCheckoutRequestedDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);

        public Task Handle(OrderPlacedDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
        
        public Task Handle(PaymentSucceededIntegrationEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
        
        public Task Handle(PaymentFailedIntegrationEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
        
        public Task Handle(ShippingGracePeriodExpiredDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
        
        public Task Handle(PaymentGracePeriodExpiredDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
        
        public Task Handle(OrderStatusSetToCancelledDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
    }
}