using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;
using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.Infrastructure.Services
{
    public class ShoppingCartOrderingService : IShoppingCartOrderingService
    {
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;

        public ShoppingCartOrderingService(IAggregateStore<ShoppingCart> shoppingCartStore)
            => _shoppingCartStore = shoppingCartStore;

        public async Task<Result<Order>> CreateOrderAsync
        (
            EntityId shoppingCartId,
            EntityId orderId,
            Guid causationId,
            Guid correlationId,
            CancellationToken cancellationToken = default
        )
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync
            (
                shoppingCartId,
                causationId,
                correlationId,
                cancellationToken
            );

            Result<Order> createOrderResult = Order.Create
            (
                orderId,
                shoppingCart.DeliveryCost,
                shoppingCart.TotalDiscount,
                shoppingCart.Customer.CustomerId,
                shoppingCart.Customer.FullName,
                shoppingCart.Customer.EmailAddress,
                shoppingCart.Customer.PhoneNumber,
                shoppingCart.Customer.DeliveryAddress,
                causationId,
                correlationId
            );
            if (createOrderResult.IsError) return createOrderResult.Error;

            Order order = createOrderResult.Data;
            foreach (ShoppingCartItem item in shoppingCart.Items)
            {
                Result addOrderLineResult = order.AddOrderLine
                (
                    item.Id,
                    item.Quantity,
                    item.UnitPrice
                );

                if (addOrderLineResult.IsError) return addOrderLineResult.Error;
            }

            return order;
        }
    }
}