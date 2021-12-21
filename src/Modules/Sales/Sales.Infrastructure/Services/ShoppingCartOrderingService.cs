﻿using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;
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
            
            Order order = new()
            {
                CorrelationId = correlationId,
                CausationId = causationId,
            };
            
            Result createOrderResult = order.Create
            (
                orderId,
                shoppingCart.DeliveryCost,
                shoppingCart.TotalDiscount,
                shoppingCart.Customer.CustomerId,
                shoppingCart.Customer.FullName,
                shoppingCart.Customer.EmailAddress,
                shoppingCart.Customer.PhoneNumber,
                shoppingCart.Customer.DeliveryAddress
            );
            
            if (createOrderResult.IsError) return createOrderResult.Error;
            
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