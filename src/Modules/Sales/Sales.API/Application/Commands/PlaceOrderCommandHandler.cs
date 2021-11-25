using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;
using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Integration.Events;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class PlaceOrderCommandHandler : ICommandHandler<PlaceOrderCommand, Order>
    {
        private readonly IAggregateRepository<Order, EntityId> _orderRepository;
        private readonly IShoppingCartOrderingService _shoppingCartOrderingService;

        public PlaceOrderCommandHandler
        (
            IAggregateRepository<Order, EntityId> orderRepository,
            IShoppingCartOrderingService shoppingCartOrderingService
        )
        {
            _orderRepository = orderRepository;
            _shoppingCartOrderingService = shoppingCartOrderingService;
        }

        public async Task<Result<Order>> Handle(PlaceOrderCommand command, CancellationToken cancellationToken)
        {
            Result<Order> createOrderResult = await _shoppingCartOrderingService.CreateOrderAsync
            (
                EntityId.Create(command.ShoppingCartId),
                EntityId.Create(command.OrderId),
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );
            
            if (createOrderResult.IsError(out ApplicationError error)) return error;

            Order order = createOrderResult.GetData();
            
            // NOTE: atomic operation
            // TODO - remove this integration event. 
            order.RaiseEvent(new OrderPlacedIntegrationEvent{ OrderId = order.Id });
            
            await _orderRepository.SaveAsync(order, cancellationToken);

            return order;
        }
    }

    public record PlaceOrderCommand : Command<Order>
    {
        public Guid OrderId { get; init; }
        public Guid ShoppingCartId { get; init; }
    }
}