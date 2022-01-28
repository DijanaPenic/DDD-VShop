using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.Modules.Sales.Domain.Enums;
using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.EventSourcing.Stores.Contracts;

namespace VShop.Modules.Sales.Infrastructure.Services
{
    internal class ShoppingCartOrderingService : IShoppingCartOrderingService
    {
        private readonly IAggregateStore<ShoppingCart> _shoppingCartStore;

        public ShoppingCartOrderingService(IAggregateStore<ShoppingCart> shoppingCartStore)
            => _shoppingCartStore = shoppingCartStore;

        public async Task<Result<Order>> CreateOrderAsync
        (
            EntityId shoppingCartId,
            CancellationToken cancellationToken = default
        )
        {
            ShoppingCart shoppingCart = await _shoppingCartStore.LoadAsync(shoppingCartId, cancellationToken);

            if (shoppingCart.Status is not ShoppingCartStatus.PendingCheckout)
                return Result.ValidationError($"Shopping cart must be in '{ShoppingCartStatus.PendingCheckout}' status.");

            Result<Order> createOrderResult = Order.Create
            (
                shoppingCart.OrderId,
                shoppingCart.DeliveryCost,
                shoppingCart.Customer.CustomerId,
                shoppingCart.Customer.Discount,
                shoppingCart.Customer.FullName,
                shoppingCart.Customer.EmailAddress,
                shoppingCart.Customer.PhoneNumber,
                shoppingCart.Customer.DeliveryAddress
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