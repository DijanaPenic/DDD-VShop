using System.Threading;
using System.Threading.Tasks;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Services;

namespace VShop.Modules.Sales.Infrastructure.Commands.Handlers
{
    internal class PlaceOrderCommandHandler : ICommandHandler<PlaceOrderCommand, Order>
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

        public async Task<Result<Order>> HandleAsync
        (
            PlaceOrderCommand command,
            CancellationToken cancellationToken
        )
        {
            Order order = await _orderStore.LoadAsync
            (
                EntityId.Create(command.OrderId).Data,
                cancellationToken
            );

            if (order is not null) return order;
            
            Result<Order> createOrderResult = await _shoppingCartOrderingService.CreateOrderAsync
            (
                EntityId.Create(command.ShoppingCartId).Data,
                cancellationToken
            );
            if (createOrderResult.IsError) return createOrderResult.Error;

            order = createOrderResult.Data;

            await _orderStore.SaveAndPublishAsync(order, cancellationToken);

            return order;
        }
    }
}