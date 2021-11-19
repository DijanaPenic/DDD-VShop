using OneOf;
using OneOf.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Integration.Events;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.SharedKernel.EventSourcing.Repositories;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class PlaceOrderCommandHandler : ICommandHandler<PlaceOrderCommand, Success<Order>>
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

        public async Task<OneOf<Success<Order>, ApplicationError>> Handle(PlaceOrderCommand command, CancellationToken cancellationToken)
        {
            OneOf<Success<Order>, ApplicationError> result = await _shoppingCartOrderingService.CreateOrderAsync
            (
                EntityId.Create(command.ShoppingCartId),
                EntityId.Create(command.OrderId),
                command.MessageId,
                command.CorrelationId
            );
            
            if (result.IsT1) return result.AsT1;

            Order order = result.AsT0.Value;
            
            // NOTE: atomic operation
            order.RaiseEvent(new OrderPlacedIntegrationEvent{ OrderId = order.Id });
            
            await _orderRepository.SaveAsync(order, cancellationToken);

            return new Success<Order>(order);
        }
    }
    
    public record PlaceOrderCommand : Command<Success<Order>>
    {
        public Guid OrderId { get; init; }
        public Guid ShoppingCartId { get; init; }
    }
}