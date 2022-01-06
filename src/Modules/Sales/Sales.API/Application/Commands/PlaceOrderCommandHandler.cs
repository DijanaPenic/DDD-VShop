using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.Domain.Models.Ordering;

namespace VShop.Modules.Sales.API.Application.Commands
{
    public class PlaceOrderCommandHandler : ICommandHandler<PlaceOrderCommand, Order>
    {
        private readonly IAggregateStore<Order> _orderStore;
        private readonly IShoppingCartOrderingService _shoppingCartOrderingService;

        public PlaceOrderCommandHandler
        (
            IAggregateStore<Order> orderStore,
            IShoppingCartOrderingService shoppingCartOrderingService
        )
        {
            _orderStore = orderStore;
            _shoppingCartOrderingService = shoppingCartOrderingService;
        }

        public async Task<Result<Order>> Handle(PlaceOrderCommand command, CancellationToken cancellationToken)
        {
            Order order = await _orderStore.LoadAsync
            (
                EntityId.Create(command.OrderId).Data,
                command.MessageId,
                command.CorrelationId,
                cancellationToken
            );

            if (order is null)
            {
                Result<Order> createOrderResult = await _shoppingCartOrderingService.CreateOrderAsync
                (
                    EntityId.Create(command.ShoppingCartId).Data,
                    EntityId.Create(command.OrderId).Data,
                    command.MessageId,
                    command.CorrelationId,
                    cancellationToken
                );
                if (createOrderResult.IsError) return createOrderResult.Error;

                order = createOrderResult.Data;
            }

            await _orderStore.SaveAndPublishAsync(order, cancellationToken);

            return order;
        }
    }

    public record PlaceOrderCommand : Command<Order>
    {
        public Guid OrderId { get; init; }
        public Guid ShoppingCartId { get; init; }
        
        public PlaceOrderCommand() { }

        public PlaceOrderCommand
        (
            Guid orderId,
            Guid shoppingCartId
        )
        {
            OrderId = orderId;
            ShoppingCartId = shoppingCartId;
        }
    }
}