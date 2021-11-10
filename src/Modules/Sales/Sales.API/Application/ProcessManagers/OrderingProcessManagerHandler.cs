using System.Threading;
using System.Threading.Tasks;

using VShop.Modules.Sales.Domain.Events;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.Infrastructure.Messaging.Events.Publishing;

namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    // TODO - should I implement my own event publisher so that I can use customer errors instead of the exception-driven approach?
    public class OrderingProcessManagerHandler :
        ProcessManagerHandler<OrderingProcessManager>,
        IDomainEventHandler<ShoppingCartCheckoutRequestedDomainEvent>,
        IDomainEventHandler<OrderPlacedDomainEvent>
    {
        public OrderingProcessManagerHandler(IProcessManagerRepository<OrderingProcessManager> processManagerRepository)
            : base(processManagerRepository)
        {
            
        }
        
        public Task Handle(ShoppingCartCheckoutRequestedDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);

        public Task Handle(OrderPlacedDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);
    }
}