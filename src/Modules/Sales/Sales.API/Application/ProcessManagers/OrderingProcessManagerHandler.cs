using Serilog;
using System.Threading;
using System.Threading.Tasks;

using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Billing.Integration.Events;
using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;

// TODO - should I implement my own event publisher so that I can use customer errors instead of the exception-driven approach?
// TODO - can PM code be simplified?
namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    internal class OrderingProcessManagerHandler :
            ProcessManagerHandler<OrderingProcessManager>,
            IDomainEventHandler<ShoppingCartCheckoutRequestedDomainEvent>,
            IDomainEventHandler<OrderPlacedDomainEvent>,
            IIntegrationEventHandler<PaymentSucceededIntegrationEvent>,
            IIntegrationEventHandler<PaymentFailedIntegrationEvent>,
            IDomainEventHandler<ShippingGracePeriodExpiredDomainEvent>,
            IDomainEventHandler<PaymentGracePeriodExpiredDomainEvent>,
            IDomainEventHandler<OrderStatusSetToCancelledDomainEvent>
    {
        public OrderingProcessManagerHandler(ILogger logger, IProcessManagerRepository<OrderingProcessManager> processManagerRepository)
            : base(logger, processManagerRepository) { }
        
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