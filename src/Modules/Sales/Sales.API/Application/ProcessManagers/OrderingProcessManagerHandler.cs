using System;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.Infrastructure.Messaging.Events.Publishing;

namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    // TODO - should I implement my own event publisher so that I can use customer errors instead of exception-driven approach?
    public class OrderingProcessManagerHandler :
        IDomainEventHandler<ShoppingCartCheckoutRequestedDomainEvent>,
        IDomainEventHandler<OrderPlacedDomainEvent>
    {
        private readonly IProcessManagerRepository<OrderingProcessManager> _processManagerRepository;
        private readonly IAggregateRepository<ShoppingCart, EntityId> _shoppingCartRepository;

        private static readonly ILogger Logger = Log.ForContext<OrderingProcessManagerHandler>();

        public OrderingProcessManagerHandler
        (
            IProcessManagerRepository<OrderingProcessManager> processManagerRepository,
            IAggregateRepository<ShoppingCart, EntityId> shoppingCartRepository
        )
        {
            _processManagerRepository = processManagerRepository;
            _shoppingCartRepository = shoppingCartRepository;
        }

        public async Task Handle(ShoppingCartCheckoutRequestedDomainEvent @event, CancellationToken _)
        {
            Logger.Information
            (
                "{Process}: handling {Event} domain event",
                nameof(OrderingProcessManagerHandler), nameof(@event)
            );
            
            ShoppingCart shoppingCart = await _shoppingCartRepository.LoadAsync(EntityId.Create(@event.ShoppingCartId));
            if (shoppingCart is null) throw new Exception("Shopping cart not found.");

            OrderingProcessManager process = new(); // TODO - I can use the same code as for update; needs refactoring
            process.Transition(@event, shoppingCart);

            await _processManagerRepository.SaveAsync(process);
        }

        public async Task Handle(OrderPlacedDomainEvent @event, CancellationToken _)
        {
            Logger.Information
            (
                "{Process}: handling {Event} domain event",
    nameof(OrderingProcessManagerHandler), nameof(@event)
            );
                
            OrderingProcessManager process = await _processManagerRepository.LoadAsync(@event.OrderId);
            if (process is null) throw new Exception("Ordering process not found.");
            
            process.Transition(@event);
        }
    }
}